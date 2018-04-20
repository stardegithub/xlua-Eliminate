using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Upload;
using Common;
using NetWork.Servers;

namespace Common.UI {

	/// <summary>
	/// 进度条提示.
	/// </summary>
	[AddComponentMenu("Common/UI/UIProgressTips")]
	public class UIProgressTips : MonoBehaviourExtension {

		/// <summary>
		/// The tip text.
		/// </summary>
		public Text _tipText = null;

		void Awake () {
			StartCoroutine (UpdateTips());
		}

		private IEnumerator UpdateTips() {

			if (this._tipText == null) {
				yield return null;
			}

			while (true) {
			
				string tip = ServersConf.GetInstance ().GetProgressTipByRandom ();
				if (string.IsNullOrEmpty (tip) == true) {
					yield break;
				}

				this._tipText.text = this.GetProgressTips(tip);
				yield return new WaitForSeconds(ServersConf.GetInstance ().ProgressTips.Interval);

			}
			yield return null;
		}

		/// <summary>
		/// 取得进度提示.
		/// </summary>
		/// <returns>进度提示.</returns>
		/// <param name="iTipsInfo">进度提示信息.</param>
		protected virtual string GetProgressTips(string iTipsInfo) {
			return iTipsInfo;
		}
	}
}
