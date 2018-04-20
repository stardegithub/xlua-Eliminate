using UnityEngine;
using System;
using System.Collections;

namespace Common {

	/// <summary>
	/// 日志类.
	/// </summary>
	public class UtilsLog {

		/// <summary>
		/// 日志No.
		/// </summary>
		private static long _logNo = -1;

		/// <summary>
		/// 已经初始化标志位.
		/// </summary>
		private static bool _inited = false;

		/// <summary>
		/// 主标签.
		/// </summary>
		private static string _mainTag = null;

		/// <summary>
		/// 初始化函数.
		/// </summary>
		private static void Init() {
			if (true == _inited) {
				return;
			}
			_inited = true;
			_logNo = 0;

			// 主标签
			_mainTag = BuildInfo.GetInstance().BuildName;
		}

		/// <summary>
		/// 取得系统日期.
		/// </summary>
		/// <returns>The current datatime.</returns>
		private static string GetCurDatatime(){
			return DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss.fff");
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Info(string iFormat, params object[] iArgs) {
			Info("-", iFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Info(string iSubTag, string iFormat, params object[] iArgs) {
			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][Info][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Warning(string iFormat, params object[] iArgs) {
			Warning("-", iFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Warning(string iSubTag, string iFormat, params object[] iArgs) {
			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][Warning][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogWarningFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Error(string iFormat, params object[] iArgs) {
			Error("-", iFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Error(string iSubTag, string iFormat, params object[] iArgs) {
			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][Error][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogErrorFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Exception(string iFormat, params object[] iArgs) {
			Exception("-", iFormat, iArgs);
		}

		/// <summary>
		/// 消息.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Exception(string iSubTag, string iFormat, params object[] iArgs) {
			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][Exception][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogErrorFormat (newFormat, iArgs);
		}
	}
}
