using UnityEngine;
using System;
using System.Collections;
using System.Globalization;

namespace Common {

	/// <summary>
	/// 时间.
	/// </summary>
	public class UtilsDateTime {

		/// <summary>
		/// 判断是否在开启日期之前.
		/// </summary>
		/// <returns><c>true</c>, 时, <c>false</c> 不是.</returns>
		/// <param name="iStartDate">开始日期（YYYY/MM/DD HH:MM）.</param>
		public static bool isBeforeDate (string iStartDate) {

			DateTime _now = DateTime.Now;
			if ((false == string.IsNullOrEmpty (iStartDate)) &&
				(false == "-".Equals (iStartDate))) {

				DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
				dtFormat.ShortDatePattern = "yyyy/MM/dd HH:MM";

				DateTime _start = Convert.ToDateTime (iStartDate, dtFormat);
				if (_now.CompareTo (_start) < 0) {
					return true;
				} 
			}
			return false;
		}

		/// <summary>
		/// 检测活动有效日期.
		/// </summary>
		/// <returns><c>true</c>, 有效期内, <c>false</c> 有效期外.</returns>
		/// <param name="iStartDate">开始日期（YYYY/MM/DD HH:MM）.</param>
		/// <param name="iEndDate">结束日期（YYYY/MM/DD HH:MM）.</param>
		public static bool CheckDate (string iStartDate, string iEndDate) {
			DateTime _now = DateTime.Now;
			if ((false == string.IsNullOrEmpty (iStartDate)) &&
				(false == "-".Equals (iStartDate))) {

				DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
				dtFormat.ShortDatePattern = "yyyy/MM/dd HH:MM";

				DateTime _start = Convert.ToDateTime (iStartDate, dtFormat);
				if (_now.CompareTo (_start) < 0) {
					return false;
				}
			}

			if ((false == string.IsNullOrEmpty (iEndDate)) &&
				(false == "-".Equals (iEndDate))) {
				DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
				dtFormat.ShortDatePattern = "yyyy/MM/dd HH:MM";

				DateTime _end = Convert.ToDateTime (iEndDate, dtFormat);
				if (_end.CompareTo (_now) <= 0) {
					return false;
				}
			}

			return true;
		}
	}
}
