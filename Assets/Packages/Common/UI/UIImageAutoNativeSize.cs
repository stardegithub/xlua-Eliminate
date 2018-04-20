using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Common {

	[RequireComponent(typeof(Image))]
	[AddComponentMenu("Common/UI/UIImageAutoNativeSize")]
	public class UIImageAutoNativeSize : MonoBehaviourExtension {

		void Start () {
			Image _image = this.GetComponent<Image> ();
			if (null != _image) {
				_image.SetNativeSize ();
			} else {
				this.Error ("Start()");
			}
		}
	}
}
