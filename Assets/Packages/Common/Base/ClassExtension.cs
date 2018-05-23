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
		/// Debug日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void DebugLog(string iFormat, params object[] iArgs);

		/// <summary>
		/// 信息日志(默认：运行日志).
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Info(string iFormat, params object[] iArgs);

		/// <summary>
		/// 警告日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Warning(string iFormat, params object[] iArgs);

		/// <summary>
		/// 信息:逻辑(LI).
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void LInfo(string iFormat, params object[] iArgs);

		/// <summary>
		/// 错误日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Error(string iFormat, params object[] iArgs);

		/// <summary>
		/// 致命日志.
		/// </summary>
		/// <param name="iScript">脚本.</param>
		/// <param name="iFormat">格式.</param>
		/// <param name="iArgs">参数.</param>
		void Fatal(string iFormat, params object[] iArgs);
	}
		
	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class ClassExtension : ILogExtension {

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

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class ObjectExtension : object, ILogExtension {

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

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class UObjectExtension : UnityEngine.Object, ILogExtension {

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

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class SObjectExtension : System.Object, ILogExtension {

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

	/// <summary>
	/// 脚本类扩展.
	/// </summary>
	public class MonoBehaviourExtension : MonoBehaviour, ILogExtension {

#if UI_DEBUG_BORDER
		private Vector3[] _uiCornersPos = new Vector3[4];
#endif

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
			
#if UI_DEBUG_BORDER

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
