using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using AssetBundles;
using Common;

public class UISceneAssetBundleLoad : MonoBehaviourExtension {

	/// <summary>
	/// 加载测试节点.
	/// </summary>
	public GameObject _loadNode = null;

	#region ButtonClick

	/// <summary>
	/// 点击返回按钮.
	/// </summary>
	public void OnBackBtnClick() {
		this.Info ("OnBackBtnClick(): -->");

		// 加载场景
		SceneManager.LoadScene ("SceneDownload");
	}

	public void OnRemoveBtnClick() {
		this.Info ("OnRemoveBtnClick(): -->");

		if (this._loadNode == null) {
			return;
		}
		for(int i = 0;i<this._loadNode.transform.childCount;i++)
		{
			GameObject child = this._loadNode.transform.GetChild(i).gameObject;
			if (child == null) {
				continue;
			}
			Destroy(child);
		}

	
	}

	/// <summary>
	/// 同步加载按钮.
	/// </summary>
	public void OnLoadSyncBtnClick() {
		this.Info ("OnLoadSyncBtnClick(): -->");
		if (this._loadNode == null) {
			return;
		}
		AudioClip audio = AssetBundlesManager.Instance.LoadAudio ("AFX_Hit_Flesh");
		if (audio != null) {
			AudioSource audioSource = this._loadNode.AddComponent<AudioSource> ();
			if (audioSource != null) {
				audioSource.PlayOneShot (audio);
			}
		}
	}

	/// <summary>
	/// 异步加载按钮.
	/// </summary>
	public void OnLoadAsyncBtnClick() {
		this.Info ("OnLoadAsyncBtnClick(): -->");
		if (this._loadNode == null) {
			return;
		}
		StartCoroutine (AssetBundlesManager.Instance.LoadAudioAsync ("AFX_Hit_GoldDrop", LoadCompletedAsyc));
	}

	public void OnLoadPrefabSyncBtnClick() {
		this.Info ("OnLoadPrefabSyncBtnClick(): -->");
		if (this._loadNode == null) {
			return;
		}
		AssetBundlesManager.Instance.LoadPrefab (
			"box", this._loadNode, 
			new Vector3(0.0f, 0.0f, 0.0f), 
			new Vector3(80.0f, 80.0f, 80.0f));
	}

	public void OnLoadPrefabAsyncBtnClick() {
		this.Info ("OnLoadPrefabAsyncBtnClick(): -->");
		if (this._loadNode == null) {
			return;
		}
		StartCoroutine (AssetBundlesManager.Instance.LoadPrefabAsync ("box", LoadCompletedAsyc));
	}

	#endregion

	private void LoadCompletedAsyc(bool iIsSuccessed, string iKey, TAssetBundleType iType, UnityEngine.Object iLoadObj) {

		this.Info ("OnLoadPrefabAsyncBtnClick(): --> IsSuccessed:{0} Key:{1} BundleType:{2}", iIsSuccessed, iKey, iType);
		if (false == iIsSuccessed) {
			return;
		}

		if (this._loadNode == null) {
			return;
		}

		switch (iType) {
		case TAssetBundleType.Prefab:
			{
				if ("box".Equals (iKey) == true) {
					GameObject prefab = GameObject.Instantiate (iLoadObj) as GameObject;
					if (prefab == null) {
						return;
					}
					prefab.transform.parent = this._loadNode.transform;
					prefab.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					prefab.transform.localScale = new Vector3 (80.0f, 80.0f, 80.0f);
				}
			}
			break;
		case TAssetBundleType.Audio:
			{
				AudioClip audio = iLoadObj as AudioClip;
				if (audio != null) {
					AudioSource audioSource = this._loadNode.AddComponent<AudioSource> ();
					if (audioSource != null) {
						audioSource.PlayOneShot (audio);
					}
				}
			}
			break;
		case TAssetBundleType.None:
		default:
			break;
		}
	}
}
