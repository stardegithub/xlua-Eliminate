using UnityEngine;
using System.Collections;

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

		/// <summary>
		/// 清空.
		/// </summary>
		public virtual void Clear() {}

		public override string ToString ()
		{
#if BUILD_DEBUG
			return string.Format ("[{0}]", this.ClassName);
#else
			return "[JsonDataBase]";
#endif
		}
	}
}
