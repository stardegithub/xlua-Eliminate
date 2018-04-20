using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Common {

	/// <summary>
	/// 编辑器基类.
	/// </summary>
	public class EditorBase : Editor, ILogExtension {

#if BUILD_DEBUG

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

#endif

		/// <summary>
		/// 日志输出：消息.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Info(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Info (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：警告.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Warning(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Warning (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：错误.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Error(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Error (this.ClassName, iFormat, iArgs);
#endif
		}

		/// <summary>
		/// 日志输出：异常.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		public void Exception(string iFormat, params object[] iArgs) {
#if BUILD_DEBUG
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}

	}
}
