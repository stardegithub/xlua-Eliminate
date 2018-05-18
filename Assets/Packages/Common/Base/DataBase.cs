using UnityEngine;
using System.Collections;
using BuildSystem.Options;

namespace Common {

	/// <summary>
	/// 设定信息.
	/// </summary>
	public class JsonDataBase : ClassExtension {

		/// <summary>
		/// 构造函数.
		/// </summary>
		public JsonDataBase () {}

		/// <summary>
		/// 清空.
		/// </summary>
		public virtual void Clear() {}

		/// <summary>
		/// 初始化.
		/// </summary>
		public virtual void Init() {}

		/// <summary>
		/// 重置.
		/// </summary>
		public virtual void Reset() {
			// 清空
			this.Clear ();
			// 初始化
			this.Init ();
		}

		public override string ToString ()
		{
#if BUILD_DEBUG
			return string.Format ("[{0}]", this.ClassName);
#else
			return "[JsonDataBase]";
#endif
		}
	}

	/// <summary>
	/// 选项数据信息.
	/// </summary>
	public class OptionsDataBase<T1, T2> : JsonDataBase
		where T1 : JsonDataBase , new() 
		where T2 : OptionsBaseData , new() {

		/// <summary>
		/// 默认数据.
		/// </summary>
		public T1 Default = new T1();

		/// <summary>
		/// 选项数据.
		/// </summary>
		public T2 Options = new T2();

		/// <summary>
		/// 构造函数.
		/// </summary>
		public OptionsDataBase () {}

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {
			base.Init ();
			Default.Init ();
			Options.Init ();
		}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			base.Clear ();

			Default.Clear ();
			Options.Clear ();
		}

		public override string ToString ()
		{
			return string.Format ("{0} Default:{1} Options:{2}", base.ToString(), 
				this.Default.ToString(), this.Options.ToString());
		}
	}
}
