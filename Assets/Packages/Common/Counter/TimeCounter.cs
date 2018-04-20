using UnityEngine;
using System.Collections;

namespace Common {

	/// <summary>
	/// 时间计数器.
	/// </summary>
	public class TimeCounter : Counter<float> {

		/// <summary>
		/// 创建时间计数器.
		/// </summary>
		/// <param name="iMaxValue">计数器最大值.</param>
		/// <param name="iOnCountOver">超过计时回调函数.</param>
		/// <param name="iMode">模式（默认为：倒计时）.</param>
		public static TimeCounter Create(
			float iMaxValue, 
			System.Action iOnCountOver = null,
			TCounterMode iMode = TCounterMode.CountDown) {
		
			TimeCounter objRet = new TimeCounter ();
			if (objRet != null) {
				objRet.InitCounter (iMaxValue, TCounterType.TimeCounter, iMode);
				if (iOnCountOver != null) {
					objRet.OnCountOver += iOnCountOver;
				}
				return objRet;
			} else {
				UtilsLog.Error ("TimeCounter", "Create():TimeCounter Create Failed!!!");
				return null;
			}
		}

		/// <summary>
		/// 构造函数（禁用外部New）.
		/// </summary>
		protected TimeCounter() {
		}

#region Implement

		/// <summary>
		/// 是否已经超过计数.
		/// </summary>
		/// <returns><c>true</c>, 已经超过计数, <c>false</c> 尚未超过计数.</returns>
		protected override bool isCountOver() {
			switch (this.Mode) {
			case TCounterMode.CountDown:
				{
					if (this.Value <= 0.0f) {
						return true;
					}
				}
				break;
			case TCounterMode.CountUp:
				{
					if (this.Value >= this.MaxValue) {
						return true;
					}
				}
				break;
			default:
				break;
			}
			return false;
		}

		/// <summary>
		/// 根据变化值更新计数器.
		/// </summary>
		/// <param name="iDeltaVaule">变化值.</param>
		protected override void UpdateCounterByDeltaValue(float iDeltaVaule) {
			
			switch (this.Mode) {
			case TCounterMode.CountDown:
				{
					this.Value -= iDeltaVaule;
				}
				break;
			case TCounterMode.CountUp:
				{
					this.Value += iDeltaVaule;
				}
				break;
			default:
				break;
			}
			this.Info ("UpdateCounterByDeltaValue():Type::{0} Mode::{1} State::{2} Value::{3}/{4}",
				this.Type, this.Mode, this.State, this.Value, this.MaxValue);
		}

#endregion
	}

}