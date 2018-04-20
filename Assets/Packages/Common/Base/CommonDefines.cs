using UnityEngine;
using System.Collections;

namespace Common {

	/// <summary>
	/// 平台类型类型.
	/// </summary>
	public enum TPlatformType {
		/// <summary>
		/// 无.
		/// </summary>
		None = 0,
		/// <summary>
		/// iOS.
		/// </summary>
		iOS = 1,
		/// <summary>
		/// 安卓：纯净版.
		/// </summary>
		Android = 2,
		/// <summary>
		/// 安卓：华为.
		/// </summary>
		Huawei = 3,
		/// <summary>
		/// 安卓：天鸽.
		/// </summary>
		Tiange = 4
	}

	/// <summary>
	/// 功能加锁类型.
	/// </summary>
	public enum TFunctionLockType {
		None,
		/// <summary>
		/// 因为运营等原因，暂时关闭(应对运营策略时使用).
		/// </summary>
		Business,
		/// <summary>
		/// 尚未开始.
		/// </summary>
		BeforeStartDate,
		/// <summary>
		/// 超出日期.
		/// </summary>
		OutOfDate,
		/// <summary>
		/// 条件解锁.
		/// </summary>
		ConditionRelease
	}

	/// <summary>
	/// 功能组.
	/// </summary>
	public enum TFunctionGroup {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// IAP相关.
		/// </summary>
		IAP,
		/// <summary>
		/// 主界面.
		/// </summary>
		Mainmenu
	}

	/// <summary>
	/// 功能解锁类型.
	/// </summary>
	public enum TFunctionReleaseType {
		/// <summary>
		/// 无.
		/// </summary>
		None,
		/// <summary>
		/// 等级解锁.
		/// </summary>
		Level,
		/// <summary>
		/// 自定义解锁.
		/// </summary>
		DIY
	}

}
