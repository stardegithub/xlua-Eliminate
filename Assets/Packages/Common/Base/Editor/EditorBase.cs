using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Common {

	/// <summary>
	/// 编辑器基类.
	/// </summary>
	public class EditorBase : Editor, ILogExtension {

		/// <summary>
		/// 类名.
		/// </summary>
		private string _className = null;
		public string ClassName {
			get { 
				if(false == string.IsNullOrEmpty(_className)) {
					return _className;
				}
				_className = GetType().Name;
				return _className;
			}
		}
			
		/// <summary>
		/// Debug日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void DebugLog(string iFormat, params object[] iArgs) {
			UtilsLog.DebugLog (this.ClassName, iFormat, iArgs);
		}

		/// <summary>
		/// 信息日志(默认：运行日志).
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Info(string iFormat, params object[] iArgs) {
			UtilsLog.Info (this.ClassName, iFormat, iArgs);
		}

		/// <summary>
		/// 警告日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Warning(string iFormat, params object[] iArgs) {
			UtilsLog.Warning (this.ClassName, iFormat, iArgs);
		}

		/// <summary>
		/// 信息:逻辑(LI).
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void LInfo(string iFormat, params object[] iArgs) {
			UtilsLog.LInfo (this.ClassName, iFormat, iArgs);
		}

		/// <summary>
		/// 错误日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Error(string iFormat, params object[] iArgs) {
			UtilsLog.Error (this.ClassName, iFormat, iArgs);
		}

		/// <summary>
		/// 致命日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Fatal(string iFormat, params object[] iArgs) {
			UtilsLog.Fatal (this.ClassName, iFormat, iArgs);
		}

	}
}
