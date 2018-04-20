using UnityEngine;
using System;
using System.Collections;

namespace Common {

	/// <summary>
	/// 计数器状态.
	/// </summary>
	public enum TCounterState {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 闲置.
		/// </summary>
		Idle,
		/// <summary>
		/// 计数中.
		/// </summary>
		Counting,
		/// <summary>
		/// 结束计数.
		/// </summary>
		End
	}

	/// <summary>
	/// 计数器类型.
	/// </summary>
	public enum TCounterType {
		/// <summary>
		/// 时间计数器.
		/// </summary>
		TimeCounter,
		/// <summary>
		/// 数字计数器.
		/// </summary>
		NumberCounter
	}

	/// <summary>
	/// 计数器模式.
	/// </summary>
	public enum TCounterMode {
		/// <summary>
		/// 倒计时.
		/// </summary>
		CountDown,
		/// <summary>
		/// 顺计时.
		/// </summary>
		CountUp
	}

	/// <summary>
	/// 计数器基类.
	/// </summary>
	public abstract class CounterBase<T> : ClassExtension {

		/// <summary>
		/// 类型.
		/// </summary>
		protected TCounterType Type = TCounterType.NumberCounter;

		/// <summary>
		/// 模式.
		/// </summary>
		protected TCounterMode Mode = TCounterMode.CountDown;

		/// <summary>
		/// 模式.
		/// </summary>
		protected TCounterState State = TCounterState.None;

		/// <summary>
		/// 空闲标志位.
		/// </summary>
		public bool IsIdle {
			get {  return (TCounterState.Idle == this.State); }
		}

		/// <summary>
		/// 循环计数标志位.
		/// </summary>
		protected bool IsLoop = false;

		/// <summary>
		/// 最大值.
		/// </summary>
		protected T MaxValue = default(T);

		/// <summary>
		/// 值.
		/// </summary>
		protected T Value = default(T);

		/// <summary>
		/// 初始化计数器.
		/// </summary>
		/// <param name="iMaxValue">最大值.</param>
		/// <param name="iType">类型.</param>
		/// <param name="iMode">模式.</param>
		public abstract void InitCounter(
			T iMaxValue, TCounterType iType, 
			TCounterMode iMode = TCounterMode.CountDown);
	
		/// <summary>
		/// 开始计数器.
		/// </summary>
		public abstract void StartCounter ();

		/// <summary>
		/// 更新计数器.
		/// </summary>
		/// <returns><c>true</c>, 已经溢出, <c>false</c> 尚未溢出.</returns>
		/// <param name="iDeltaValue">变化值.</param>
		public abstract bool UpdateCounter (T iDeltaValue);

		/// <summary>
		/// 结束计数器.
		/// </summary>
		public abstract void EndCounter ();

		/// <summary>
		/// 重置计数器.
		/// </summary>
		public abstract void ResetCounter ();

		/// <summary>
		/// 重启计数器.
		/// </summary>
		public abstract void RestartCounter ();

		/// <summary>
		/// 取得计数器值（根据类型不同，格式也不同）.
		/// </summary>
		public abstract string GetCounterValue ();

	}

	/// <summary>
	/// 计数器.
	/// </summary>
	public abstract class Counter<T> : CounterBase<T> {

		protected System.Action _countOver = () => {};
		/// <summary>
		/// 超过计时回调函数.
		/// </summary>
		protected event System.Action OnCountOver {
			add { _countOver += value; } remove { _countOver -= value; }
		}

		#region abstract

		/// <summary>
		/// 是否已经超过计数.
		/// </summary>
		/// <returns><c>true</c>, 已经超过计数, <c>false</c> 尚未超过计数.</returns>
		protected abstract bool isCountOver();

		/// <summary>
		/// 根据变化值更新计数器.
		/// </summary>
		/// <param name="iDeltaVaule">变化值.</param>
		protected abstract void UpdateCounterByDeltaValue(T iDeltaVaule);

		#endregion

		#region Implement

		/// <summary>
		/// 初始化计数器.
		/// </summary>
		/// <param name="iMaxValue">最大值.</param>
		/// <param name="iType">类型.</param>
		/// <param name="iMode">模式.</param>
		public override void InitCounter(
			T iMaxValue, 
			TCounterType iType, 
			TCounterMode iMode = TCounterMode.CountDown) {
			
			this.MaxValue = iMaxValue;
			this.Type = iType;
			this.Mode = iMode;

			this.State = TCounterState.Idle;

			switch (this.Mode) {
			case TCounterMode.CountDown:
				{
					this.Value = this.MaxValue;
				}
				break;
			case TCounterMode.CountUp:
				{
					this.Value = default(T);
				}
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// 开始计数器.
		/// </summary>
		public override void StartCounter () {
			this.State = TCounterState.Counting;
			this.Info ("StartCounter():Type::{0} Mode::{1} State::{2} Value::{3}/{4}",
				this.Type, this.Mode, this.State, this.Value, this.MaxValue);
		}

		/// <summary>
		/// 更新计数器.
		/// </summary>
		/// <param name="iDeltaValue">变化值.</param>
		public override bool UpdateCounter (T iDeltaValue) {

			if (TCounterState.Counting != this.State) {
				return false;
			}
			if (this.isCountOver () == true) {
				this._countOver ();
				this.EndCounter ();
				return true;
			} else {
				this.State = TCounterState.Counting;
			}

			// 根据变化值更新计数器
			this.UpdateCounterByDeltaValue (iDeltaValue);

			return false;
		}

		/// <summary>
		/// 结束计数器.
		/// </summary>
		public override void EndCounter () {
			if (TCounterState.Counting == this.State) {
				this.Info ("EndCounter():Type::{0} Mode::{1} State::{2} Value::{3}/{4}",
					this.Type, this.Mode, this.State, this.Value, this.MaxValue);
			}
			this.State = TCounterState.End;
		}

		/// <summary>
		/// 重置计数器.
		/// </summary>
		public override void ResetCounter () {
			this.InitCounter (this.MaxValue, this.Type, this.Mode);
		}
			
		/// <summary>
		/// 重启计数器.
		/// </summary>
		public override void RestartCounter () {
			// 重置计数器
			this.ResetCounter();
			// 开始计数
			this.StartCounter();
			this.Info ("RestartCounter():Type::{0} Mode::{1} State::{2} Value::{3}/{4}",
				this.Type, this.Mode, this.State, this.Value, this.MaxValue);
		}

		/// <summary>
		/// 取得计数器值（根据类型不同，格式也不同）.
		/// </summary>
		public override string GetCounterValue () {
			return this.Value.ToString();
		}

		#endregion
	};
}
