using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Common;

namespace UnityHttp
{
    public delegate void HttpResponseHandler(bool isSuccess, HttpStatusCode httpStatusCode, string response);

    /// <summary>
    /// http请求类，基于C#的HttpWebRequest实现
    /// </summary>
    public class HttpRequest : ClassExtension
    {
        private int retryCount = 1;
        private byte[] paramBytes;

        /// <summary>
        /// http同步请求结束回调
        /// </summary>
        public event HttpResponseHandler httpRequestCallback;

        /// <summary>
        /// http异步请求结束回调
        /// </summary>
        public event HttpResponseHandler asyncHttpRequestCallback;

        /// <summary>
        /// 创建http同步请求
        /// </summary>
        /// <param name="requestInfo">请求信息</param>
        public void CreateHttpRequest(HttpRequestInfo requestInfo)
        {
            Info("HttpRequest[{0}]: {1}", retryCount, requestInfo);
            // ignore SSL certificate errors
            ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;

            HttpWebRequest request = null;
            try
            {
                paramBytes = Encoding.UTF8.GetBytes(requestInfo.Param);
                request = HttpWebRequest.Create(requestInfo.URL) as HttpWebRequest;
                request.Method = requestInfo.Method;
                request.ContentType = "application/json";
                request.ContentLength = paramBytes.Length;
                foreach (var element in requestInfo.Headers)
                {
                    request.Headers.Add(element.Key, element.Value);
                }
                request.Timeout = 5000;
                request.KeepAlive = false;
                if (requestInfo.Method == "POST" && paramBytes.Length > 0)
                {
                    using (Stream writer = request.GetRequestStream())
                    {
                        writer.Write(paramBytes, 0, paramBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Error("retryCount={0}, error={1}", retryCount, e.Message);
                request.Abort();
                RetryRequest(requestInfo, HttpStatusCode.BadRequest, e.Message);
                return;
            }

            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseJson = reader.ReadToEnd();

                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    Info("HttpResponse[{0}]: {1},{2},{3}", retryCount, "true", response.StatusCode, responseJson);
                    retryCount = 1;
                    httpRequestCallback.Invoke(true, response.StatusCode, responseJson);
                }
                else
                {
                    Info("retryCount={0}, status={1}, response={2}", retryCount, response.StatusCode, responseJson);
                    request.Abort();
                    RetryRequest(requestInfo, response.StatusCode, responseJson);
                }
                reader.Close();
                response.Close();
            }
            catch (Exception e)
            {
                Error("HttpResponse[{0}]: {1},{2},{3}", "false", retryCount, HttpStatusCode.ExpectationFailed, e.Message);
                retryCount = 1;
                request.Abort();
                httpRequestCallback.Invoke(false, HttpStatusCode.ExpectationFailed, e.Message);
            }
        }

        /// <summary>
        /// 创建http异步请求
        /// </summary>
        /// <param name="requestInfo">请求信息</param>
        public void CreateAsyncHttpRequest(HttpRequestInfo requestInfo)
        {
            Info("HttpRequest[{0}]: {1}", retryCount, requestInfo);
            // ignore SSL certificate errors
            ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;

            paramBytes = Encoding.UTF8.GetBytes(requestInfo.Param);
            HttpWebRequest request = HttpWebRequest.Create(requestInfo.URL) as HttpWebRequest;
            request.Method = requestInfo.Method;
            request.ContentType = "application/json";
            request.ContentLength = paramBytes.Length;
            foreach (var element in requestInfo.Headers)
            {
                request.Headers.Add(element.Key, element.Value);
            }
            request.Timeout = 5000;
            request.KeepAlive = false;
            if (requestInfo.Method == "POST")
            {
                request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
            }
            else
            {
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }
        }

        private void GetRequestStreamCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            using (Stream writer = request.EndGetRequestStream(asyncResult))
            {
                writer.Write(paramBytes, 0, paramBytes.Length);
            }
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private void GetResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            HttpWebResponse response = request.EndGetResponse(asyncResult) as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseJson = reader.ReadToEnd();
            reader.Close();
            if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
            {
                asyncHttpRequestCallback.Invoke(true, response.StatusCode, responseJson);
            }
            else
            {
                asyncHttpRequestCallback.Invoke(false, response.StatusCode, responseJson);
            }
            response.Close();
        }

        private void RetryRequest(HttpRequestInfo requestInfo, HttpStatusCode httpStatusCode, string error)
        {
            retryCount = retryCount + 1;
            if (retryCount > 3)
            {
                Info("HttpResponse[{0}]: {1},{2},{3}", retryCount - 1, "false", httpStatusCode, error);
                retryCount = 1;
                httpRequestCallback.Invoke(false, httpStatusCode, error);
            }
            else
            {
                CreateHttpRequest(requestInfo);
            }
        }
    }

    /// <summary>
    /// http请求消息类
    /// </summary>
    public class HttpRequestInfo
    {
        /// <summary>
        /// 地址
        /// </summary>
        /// <returns></returns>
        public string URL { get; set; }

        /// <summary>
        /// 请求方法（post，get）
        /// </summary>
        /// <returns></returns>
        public string Method { get; set; }

        private Dictionary<string, string> param;
        /// <summary>
        /// 参数
        /// </summary>
        public string Param
        {
            get
            {
                if (param.Count == 0) return "";
                StringBuilder sb = new StringBuilder();
                sb.Append('{');
                if (param.Count > 0)
                {
                    foreach (var element in param)
                    {
                        sb.Append(string.Format("\"{0}\":\"{1}\",", element.Key, element.Value));
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append('}');
                return sb.ToString();
            }
        }

        private Dictionary<string, string> headers;
        /// <summary>
        /// 消息头
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Headers { get { return headers; } }

        public HttpRequestInfo()
        {
            URL = "";
            Method = "";
            headers = new Dictionary<string, string>();
            param = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加请求参数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void AddParam(string key, string value)
        {
            param.Add(key, value);
        }

        /// <summary>
        /// 添加消息头
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void AddHeader(string key, string value)
        {
            headers.Add(key, value);
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("url:{0},method:{1},content:{2}", URL, Method, Param);
        }
    }
}
