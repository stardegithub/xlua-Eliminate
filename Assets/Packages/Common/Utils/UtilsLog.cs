using UnityEngine;
using System;
using System.Collections;

namespace Common {

	/// <summary>
	/// Log日志等级定义（级别越高，日志输出越少。）.
	/// 输出的日志只输出大于等于当前日志级别的日志。
	/// </summary>
	public enum LogLevel {
		Invalid = -1,
		/// <summary>
		/// 全部输出(A).
		/// </summary>
		All = 0,
		/// <summary>
		/// Debug(D).
		/// </summary>
		Debug = 1,
		/// <summary>
		/// 信息:运行(RI).
		/// </summary>
		RInfo = 2,
		/// <summary>
		/// 警告(W).
		/// </summary>
		Warning = 3,
		/// <summary>
		/// 信息:逻辑(LI).
		/// </summary>
		LInfo = 4,
		/// <summary>
		/// 错误(E).
		/// </summary>
		Error = 5,
		/// <summary>
		/// 致命日志(F).
		/// </summary>
		Fatal = 6,
		/// <summary>
		/// 全关闭.
		/// </summary>
		Off = 7
	}

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
		/// 主标签.
		/// </summary>
		private static LogLevel _curLogLevel = LogLevel.Invalid;

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

			// 当前日志等级
			_curLogLevel = BuildInfo.GetInstance().LogLevel;
		}

		/// <summary>
		/// 取得系统日期.
		/// </summary>
		/// <returns>The current datatime.</returns>
		private static string GetCurDatatime(){
			return DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss.fff");
		}

		/// <summary>
		/// Debug日志.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void DebugLog(string iSubTag, string iFormat, params object[] iArgs) {

			// 判断日志级别
			if (_curLogLevel >= LogLevel.Debug) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][D][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogFormat (newFormat, iArgs);
			
		}

		/// <summary>
		/// 信息日志.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Info(string iSubTag, string iFormat, params object[] iArgs) {
			RInfo (iSubTag, iFormat, iArgs);
		}

		/// <summary>
		/// 信息:运行(RI).
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void RInfo(string iSubTag, string iFormat, params object[] iArgs) {

			// 判断日志级别
			if (_curLogLevel >= LogLevel.RInfo) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][RI][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 警告日志.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Warning(string iSubTag, string iFormat, params object[] iArgs) {
			
			// 判断日志级别
			if (_curLogLevel >= LogLevel.Warning) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][W][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogWarningFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 信息:逻辑(LI).
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void LInfo(string iSubTag, string iFormat, params object[] iArgs) {

			// 判断日志级别
			if (_curLogLevel >= LogLevel.LInfo) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][LI][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 错误日志.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Error(string iSubTag, string iFormat, params object[] iArgs) {

			// 判断日志级别
			if (_curLogLevel >= LogLevel.Error) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][E][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogErrorFormat (newFormat, iArgs);
		}

		/// <summary>
		/// 致命日志.
		/// </summary>
		/// <param name="iSubTag">次标签.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数列表.</param>
		public static void Fatal(string iSubTag, string iFormat, params object[] iArgs) {

			// 判断日志级别
			if (_curLogLevel >= LogLevel.Fatal) {
				return;
			}

			// 初始化
			Init();

			++_logNo;
			string dateTime = GetCurDatatime ();
			string newFormat = string.Format ("{0}: [{1}][{2}][F][{3}] {4}", _mainTag, dateTime, _logNo, iSubTag, iFormat);
			Debug.LogErrorFormat (newFormat, iArgs);
		}
	}
}
