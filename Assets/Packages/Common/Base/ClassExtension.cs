using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

namespace Common {

	/// <summary>
	/// 日志类扩展接口.
	/// </summary>
	public interface ILogExtension {
		/// <summary>
		/// 日志输出：消息.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Info(string iFormat, params object[] iArgs);

		/// <summary>
		/// 日志输出：警告.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Warning(string iFormat, params object[] iArgs);

		/// <summary>
		/// 日志输出：错误.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Error(string iFormat, params object[] iArgs);

		/// <summary>
		/// 日志输出：异常.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Exception(string iFormat, params object[] iArgs);
	}
		
	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class ClassExtension : ILogExtension {

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}

//	/// <summary>
//	/// 脚本类扩展.
//	/// </summary>
//	public class ClassExtension<T> : T, ILogExtension
//		where T : class, new() {
//
//#if BUILD_DEBUG
//
//		/// <summary>
//		/// 日志输出标志位.
//		/// </summary>
//		protected bool _logOutput = true;
//
//		/// <summary>
//		/// 类名.
//		/// </summary>
//		private string _className = null;
//		public string ClassName {
//		get { 
//		if(false == string.IsNullOrEmpty(_className)) {
//		return _className;
//		}
//		_className = GetType().Name;
//		return _className;
//		}
//		}
//
//#endif
//
//		/// <summary>
//		/// 日志输出：消息.
//		/// </summary>
//		/// <param name="iScript">脚本.</param>
//		/// <param name="iFormat">格式.</param>
//		/// <param name="iArgs">参数.</param>
//		public void Info(string iFormat, params object[] iArgs) {
//#if BUILD_DEBUG
//			if(false == this._logOutput) {
//			return;
//			}
//			UtilsLog.Info (this.ClassName, iFormat, iArgs);
//#endif
//		}
//
//		/// <summary>
//		/// 日志输出：警告.
//		/// </summary>
//		/// <param name="iScript">脚本.</param>
//		/// <param name="iFormat">格式.</param>
//		/// <param name="iArgs">参数.</param>
//		public void Warning(string iFormat, params object[] iArgs) {
//#if BUILD_DEBUG
//			if(false == this._logOutput) {
//			return;
//			}
//			UtilsLog.Warning (this.ClassName, iFormat, iArgs);
//#endif
//		}
//
//		/// <summary>
//		/// 日志输出：错误.
//		/// </summary>
//		/// <param name="iScript">脚本.</param>
//		/// <param name="iFormat">格式.</param>
//		/// <param name="iArgs">参数.</param>
//		public void Error(string iFormat, params object[] iArgs) {
//#if BUILD_DEBUG
//			if(false == this._logOutput) {
//			return;
//			}
//			UtilsLog.Error (this.ClassName, iFormat, iArgs);
//#endif
//		}
//
//		/// <summary>
//		/// 日志输出：异常.
//		/// </summary>
//		/// <param name="iScript">脚本.</param>
//		/// <param name="iFormat">格式.</param>
//		/// <param name="iArgs">参数.</param>
//		public void Exception(string iFormat, params object[] iArgs) {
//#if BUILD_DEBUG
//			if(false == this._logOutput) {
//			return;
//			}
//			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
//#endif
//		}
//	}

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class ObjectExtension : object, ILogExtension {

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class UObjectExtension : UnityEngine.Object, ILogExtension {

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class SObjectExtension : System.Object, ILogExtension {

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class MonoBehaviourExtension : MonoBehaviour, ILogExtension {

#if UI_DEBUG
		private Vector3[] _uiCornersPos = new Vector3[4];
#endif

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}

#if UI_DEBUG

		/// <summary>
		/// 绘制UI边界线.
		/// </summary>
		public void OnDrawGizmos()
		{
			foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
			{
				if (g.raycastTarget)
				{
					RectTransform rectTransform = g.transform as RectTransform;
					rectTransform.GetWorldCorners(_uiCornersPos);
					Gizmos.color = Color.blue;
					for (int i = 0; i < 4; i++)
						Gizmos.DrawLine(_uiCornersPos[i], _uiCornersPos[(i + 1) % 4]);

				}
			}
		}

#endif
	}

	public class ScriptableObjectExtension : ScriptableObject, ILogExtension {

#if BUILD_DEBUG

		/// <summary>
		/// 日志输出标志位.
		/// </summary>
		protected bool _logOutput = true;

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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
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
			if(false == this._logOutput) {
				return;
			}
			UtilsLog.Exception (this.ClassName, iFormat, iArgs);
#endif
		}
	}

}
