using System;
using System.Text;
using System.Collections;
using Common;

namespace AndroidSDK.Options.OneSDK {

	/// <summary>
	/// 易接专用JSON对象.
	/// </summary>
	public sealed class SFJSONObject :ClassExtension
	{
		public Hashtable nameValuePairs = new Hashtable(); 
		private string ins;
		private char[] cins;
		private int pos;
		public SFJSONObject() {
		}
		public SFJSONObject(String json) {
			JSONTokener readFrom = new JSONTokener(json);
			Object obj = readFrom.nextValue();

			if(obj is SFJSONObject){
				this.nameValuePairs = ((SFJSONObject)obj).nameValuePairs;
			}


		}
		public object get(string name){
			return nameValuePairs[name];
		}
		public void put(string key, object value){
			nameValuePairs.Add(key,value);
		}


		public string toString()
		{
			string svalue = "{";
			foreach (DictionaryEntry de in nameValuePairs)
			{
				svalue += "\""+de.Key+"\""+":"+"\""+de.Value+"\""+",";
			}

			svalue = svalue.Remove(svalue.Length-1);
			svalue += "}";

			return svalue;
		}

		public string toInlineString()
		{
			string svalue = "{";
			foreach (DictionaryEntry de in nameValuePairs)
			{
				svalue += "\\\""+de.Key+"\\\""+":"+"\\\""+de.Value+"\\\""+",";
			}

			svalue = svalue.Remove(svalue.Length-1);
			svalue += "}";

			return svalue;
		}

	}
}