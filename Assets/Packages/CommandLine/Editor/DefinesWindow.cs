using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace CommandLine {

	/// <summary>
	/// 自定义宏配置信息.
	/// </summary>
	[System.Serializable]
	public class DefinesConfInfo : WindowConfInfoBase {

		/// <summary>
		/// 输入项：新追加宏定义.
		/// </summary>
		public string InputNewDefine;

		/// <summary>
		/// 输入项：安卓.
		/// </summary>
		public bool InputAndroidSelected;

		/// <summary>
		/// 输入项：iOS.
		/// </summary>
		public bool InputIOSSelected;

		/// <summary>
		/// 列表显示范围.
		/// </summary>
		public Rect ScrollViewRect;

		#region Implement

		/// <summary>
		/// 初始化.
		/// </summary>
		public override  void Init() {
			this.InputNewDefine = null;
			this.InputAndroidSelected = false;
			this.InputIOSSelected = false;
			this.WindowName = "自定义宏追加";
			this.Title = "宏一览";
			this.LineHeight = 20.0f;
			this.DisplayRect = new Rect (0.0f, 0.0f, 400.0f, 200.0f);
		}

		#endregion
	}

	/// <summary>
	/// 宏定义.
	/// </summary>
	[System.Serializable]
	public class DefineInfo {
		public string Name;
		public bool Android;
		public bool iOS;

		public DefineInfo(string iName, bool iAndroid, bool iIOS) {
			this.Name = iName;
			this.Android = iAndroid;
			this.iOS = iIOS;
		}
	}

	/// <summary>
	/// 自定义宏数据.
	/// </summary>
	[System.Serializable]
	public class DefinesData : WindowDataBase {
		
		/// <summary>
		/// XCode工程设定情报.
		/// </summary>
		public List<DefineInfo> Defines = new List<DefineInfo> ();

		/// <summary>
		/// 初始化.
		/// </summary>
		public override void Init() {

			if (null != this.Defines) {
				this.Defines.Clear ();
			}
		}

		/// <summary>
		/// 重置.
		/// </summary>
		public override void Reset() {}

		/// <summary>
		/// 清空.
		/// </summary>
		public override void Clear() {
			this.Init ();
		}
			
		/// <summary>
		/// 追加宏.
		/// </summary>
		/// <param name="iAddNewDefine">新追加的宏.</param>
		public void AddDefines(string[] iNames, bool iAndroid, bool iIOS) {

			if ((null == iNames) || (0 >= iNames.Length) ||
				(null == this.Defines)) {
				return;
			}
			foreach (string defineName in iNames) {
				bool isExist = false;
				foreach (DefineInfo define in this.Defines) {
					if (true == string.IsNullOrEmpty (define.Name)) {
						continue;
					}
					if (true == defineName.Equals (define.Name)) {
						if (true == iAndroid) {
							define.Android = iAndroid;
						}
						if (true == iIOS) {
							define.iOS = iIOS;
						}
						isExist = true;
						break;
					}
				}
				if (false == isExist) {
					DefineInfo _define = new DefineInfo (defineName, iAndroid, iIOS);
					this.Defines.Add (_define);
					UtilsLog.Info ("DefinesData", "AddDefine -> Name:{0} Andorid:{1} iOS:{2}", 
						defineName, iAndroid, iIOS);
				}
			}

			// 按名字排序
			this.Defines.Sort((x,y) => (x.Name.CompareTo(y.Name)));

		}

		/// <summary>
		/// 应用.
		/// </summary>
		public override void Apply() {

			// 安卓设定
			DefineInfo[] _androids = this.Defines
				.Where (o => (true == o.Android))
				.OrderBy (o => o.Name)
				.ToArray ();
			DefinesSetting.SetDefines (_androids, BuildTargetGroup.Android);

			// iOS设定
			DefineInfo[] _iOSs = this.Defines
				.Where (o => (true == o.iOS))
				.OrderBy (o => o.Name)
				.ToArray ();
			DefinesSetting.SetDefines (_iOSs, BuildTargetGroup.iOS);
		}
	}

	/// <summary>
	/// 自定义宏窗口.
	/// </summary>
	public class DefinesWindow : WindowBase<DefinesData, DefinesConfInfo> {

		public static readonly string _jsonFileDir = "Assets/Packages/CommandLine/Editor/Json";

		/// <summary>
		/// 输入项：新追加宏定义.
		/// </summary>
		private static string InputNewDefine {
			get {
				if(null == ConfInfo) {
					return null;
				}
				return ConfInfo.InputNewDefine;
			}	
			set { 
				if(null == ConfInfo) {
					return;
				}
				ConfInfo.InputNewDefine = value;
			}
		}

		/// <summary>
		/// 输入项：安卓.
		/// </summary>
		private static bool InputAndroidSelected {
			get {
				if(null == ConfInfo) {
					return false;
				}
				return ConfInfo.InputAndroidSelected;
			}	
			set { 
				if(null == ConfInfo) {
					return;
				}
				ConfInfo.InputAndroidSelected = value;
			}
		}

		/// <summary>
		/// 输入项：iOS.
		/// </summary>
		private static bool InputIOSSelected {
			get {
				if(null == ConfInfo) {
					return false;
				}
				return ConfInfo.InputIOSSelected;
			}	
			set { 
				if(null == ConfInfo) {
					return;
				}
				ConfInfo.InputIOSSelected = value;
			}
		}

		/// <summary>
		/// 列表显示范围.
		/// </summary>
		private static Rect ScrollViewRect {
			get {
				if(null == ConfInfo) {
					return new Rect(0.0f, 0.0f, 100.0f, 100.0f);
				}
				return ConfInfo.ScrollViewRect;
			}	
			set { 
				if(null == ConfInfo) {
					return;
				}
				ConfInfo.ScrollViewRect = value;
			}
		}

		/// <summary>
		/// 滚动列表开始位置.
		/// </summary>
		private Vector2 _scrollViewStartPos = Vector2.zero;

		/// <summary>
		/// 自定义宏一览.
		/// </summary>
		public List<DefineInfo> Defines {
			get { 
				if (null == this.Data) {
					return null;
				}
				return this.Data.Defines;
			}
			private set {
				if (null == this.Data) {
					return;
				}
				this.Data.Defines = value;
			}
		}

		/// <summary>
		/// 显示宏定义窗口.
		/// </summary>
		[UnityEditor.MenuItem("Tools/Defines")]
		static void ShowDefinesWindow() {

			//创建窗口	
			DefinesWindow window = UtilsWindow.CreateWindow<DefinesWindow, DefinesConfInfo>(DefinesWindow.ConfInfo);	
			if (null == window) {
				UtilsLog.Error ("DefinesWindow", "ShowDefinesWindow -> Create Window Failed!!");
				return;
			}
			window.Show();
		}
	
        /// <summary>
        /// 构造函数.
        /// </summary>
        // public DefinesWindow() {
        // 	if (false == this.Init (_jsonFileDir)) {
        // 		UtilsLog.Error (this.ClassName, "DefinesWindow Failed!!!");
        // 	}
        // }

        /// <summary>
        /// 窗口类不要写构造函数，初始化写在Awake里
        /// </summary>
        void Awake()
        {
            if (false == this.Init(_jsonFileDir))
            {
				UtilsLog.Error(this.ClassName, "Awake()::DefinesWindow Failed!!!");
            }
        }   

		/// <summary>
		/// 追加宏.
		/// </summary>
		/// <param name="iAddNewDefine">新追加的宏.</param>
		private void AddDefine(string iName, bool iAndroid, bool iIOS) {
		
			if ((true == string.IsNullOrEmpty (iName)) || (null == this.Defines)) {
				return;
			}
			bool isExist = false;
			foreach (DefineInfo define in this.Data.Defines) {
				if (true == string.IsNullOrEmpty (define.Name)) {
					continue;
				}
				if (true == iName.Equals (define.Name)) {
					define.Android = iAndroid;
					define.iOS = iIOS;
					isExist = true;
					break;
				}
			}
			if (false == isExist) {
				DefineInfo _define = new DefineInfo (iName, iAndroid, iIOS);
				this.Defines.Add (_define);
			}

			// 按名字排序
			this.Defines.Sort((x,y) => (x.Name.CompareTo(y.Name)));

			UtilsLog.Info ("DefinesWindow", "AddDefine -> Name:{0} Andorid:{1} iOS:{2}", 
				iName, iAndroid, iIOS);

		}

		/// <summary>
		/// 删除宏.
		/// </summary>
		/// <param name="iDelDefine">删除的宏索引.</param>
		private void DelDefine(int iDelDefineIdx) {

			if ((-1 >= iDelDefineIdx) || (null == this.Defines)) {
				return;
			}
			isPause = true;
			DefineInfo _delDefine = this.Defines[iDelDefineIdx];
			this.Defines.RemoveAt (iDelDefineIdx);
		
			// 按名字排序
			this.Defines.Sort((x,y) => (x.Name.CompareTo(y.Name)));

			UtilsLog.Info (this.ClassName, "DelDefine -> Name:{0} Andorid:{1} iOS:{2}", 
				_delDefine.Name, _delDefine.Android, _delDefine.iOS);
			isPause = false;
		}
			
		#region Implement

		/// <summary>
		/// 清空按钮点击事件.
		/// </summary>
		protected override void OnClearClick() {
			this.isPause = true;
			base.OnClearClick ();
			ConfInfo.InputNewDefine = null;
			this.isPause = false;
		}

		/// <summary>
		/// 初始化窗口尺寸信息.
		/// </summary>
		/// <param name="iDisplayRect">表示范围.</param>
		protected override void InitWindowSizeInfo(Rect iDisplayRect) {
			base.InitWindowSizeInfo (iDisplayRect);
			Rect _ScrollViewRect = iDisplayRect;

			// 标题行
			_ScrollViewRect.y += LineHeight;
			_ScrollViewRect.height -= LineHeight;

			// 输入框
			_ScrollViewRect.height -= LineHeight;

			// 清空，导入，导出按钮行
			_ScrollViewRect.height -= LineHeight * 1.5f;

			ScrollViewRect = _ScrollViewRect;

			UtilsLog.Info (this.ClassName, "InitWindowSizeInfo ScrollViewRect(X:{0} Y:{1} Width:{2} Height:{3})",
				ScrollViewRect.x, ScrollViewRect.y, ScrollViewRect.width, ScrollViewRect.height);

		}

		/// <summary>
		/// 绘制WindowGUI.
		/// </summary>
		protected override void OnWindowGUI () {

			// 列表
			_scrollViewStartPos = EditorGUILayout.BeginScrollView (
				_scrollViewStartPos, GUILayout.Width(ScrollViewRect.width), GUILayout.Height(ScrollViewRect.height));
			if(null != this.Defines) {
				for(int idx = 0; idx < this.Defines.Count; ++idx) {
					DefineInfo defineInfo = this.Defines[idx];
					if (true == string.IsNullOrEmpty (defineInfo.Name)) {
						continue;
					}
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (defineInfo.Name, 
						GUILayout.Width(ScrollViewRect.width - 175.0f), GUILayout.Height(LineHeight));

					EditorGUILayout.LabelField ("Android", 
						GUILayout.Width(48.0f), GUILayout.Height(LineHeight));
					defineInfo.Android = EditorGUILayout.Toggle(defineInfo.Android, 
						GUILayout.Width(10.0f), GUILayout.Height(LineHeight));

					EditorGUILayout.LabelField ("iOS", 
						GUILayout.Width(22.0f), GUILayout.Height(LineHeight));
					defineInfo.iOS = EditorGUILayout.Toggle(defineInfo.iOS, 
						GUILayout.Width(10.0f), GUILayout.Height(LineHeight));

					if(GUILayout.Button("Del",GUILayout.Width(40.0f)))
					{
						// 删除宏
						this.DelDefine(idx);
						InputNewDefine = null;
					}

					EditorGUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.EndScrollView ();

			// 追加宏
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("追加宏:", 
				GUILayout.Width(40.0f), GUILayout.Height(LineHeight));
			InputNewDefine = EditorGUILayout.TextField (
				InputNewDefine, GUILayout.Width(195.0f), GUILayout.Height(ConfInfo.LineHeight));
			     
			EditorGUILayout.LabelField ("Android", 
				GUILayout.Width(48.0f), GUILayout.Height(LineHeight));
			InputAndroidSelected = EditorGUILayout.Toggle(InputAndroidSelected, 
				GUILayout.Width(10.0f), GUILayout.Height(LineHeight));

			EditorGUILayout.LabelField ("iOS", 
				GUILayout.Width(22.0f), GUILayout.Height(LineHeight));
			InputIOSSelected = EditorGUILayout.Toggle(InputIOSSelected, 
				GUILayout.Width(10.0f), GUILayout.Height(LineHeight));

			if(GUILayout.Button("Add",GUILayout.Width(40.0f)))
			{
				// 追加宏
				this.AddDefine(InputNewDefine, InputAndroidSelected, InputIOSSelected);
				InputNewDefine = null;
				InputAndroidSelected = false;
				InputIOSSelected = false;
			}
			EditorGUILayout.EndHorizontal ();

			this.Repaint ();
		}

		/// <summary>
		/// 应用导入数据数据.
		/// </summary>
		/// <param name="iData">数据.</param>
		/// <param name="iForceClear">强制清空标志位.</param>
		protected override void ApplyImportData (DefinesData iData, bool iForceClear) {

			if (null == iData) {
				return;
			}

			// 清空
			if (true == iForceClear) {
				this.Clear ();
			}
			this.Data.Defines.AddRange(iData.Defines);
			this.Data.Defines.Sort((x,y) => (x.Name.CompareTo(y.Name)));

			UtilsAsset.SetAssetDirty (this);
		}

		#endregion
	}
}
