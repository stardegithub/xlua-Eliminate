using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common {

	/// <summary>
	/// Json工具.
	/// </summary>
	public class UtilsJson<T> : ClassExtension 
		where T : JsonDataBase, new() {

		/// <summary>
		/// 将Json字符串转换成对象.
		/// </summary>
		/// <returns>对象.</returns>
		/// <param name="iJsonStr">Json字符串.</param>
		public static T ConvertFromJsonString(string iJsonStr) {
			if (true == string.IsNullOrEmpty (iJsonStr)) {
				return default(T);
			}
			return JsonUtility.FromJson<T>(iJsonStr);
		}

		/// <summary>
		/// 将对象转换成Json字符串.
		/// </summary>
		/// <returns>Json字符串.</returns>
		/// <param name="iObject">对象.</param>
		public static string ConvertToJsonString(T iObject) {
			if (default(T) == iObject) {
				return null;
			}
			return JsonUtility.ToJson(iObject);
		} 
	}
}
