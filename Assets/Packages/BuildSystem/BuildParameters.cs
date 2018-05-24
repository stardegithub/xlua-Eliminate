using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Common;

namespace BuildSystem {
	
	/// <summary>
	/// Build Options.
	/// </summary>
	public class BuildParameters  {

		/// <summary>
		/// 输出目录(格式：-output dir).
		/// </summary>
		/// <value>输出目录(格式：-output dir).</value>
		public static string OutputDir {
			get { 

				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-output") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// 项目名称(格式：-projectName name).
		/// </summary>
		/// <value>项目名称(格式：-projectName name).</value>
		public static string ProjectName
		{
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-projectName") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// 项目名称(格式：-gameName name).
		/// </summary>
		/// <value>项目名称(格式：-gameName name).</value>
		public static string GameName
		{
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-gameName") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}


		/// <summary>
		/// Build ID(格式：-buildId ID).
		/// </summary>
		/// <value>Build ID(格式：-buildId ID).</value>
		public static string BuildId {
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-buildId") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Build Version(格式：-buildVersion version).
		/// </summary>
		/// <value>Build Version(格式：-buildVersion version).</value>
		public static string BuildVersion {
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-buildVersion") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Build Version Code(格式：-buildVersionCode version).
		/// </summary>
		/// <value>Build Version Code(格式：-buildVersionCode version).</value>
		public static int BuildVersionCode {
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return 1;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-buildVersionCode") && (args.Length > idx + 1))
					{
						string _value = args [idx + 1];
						if(true == string.IsNullOrEmpty(_value)) {
							return 1;
						}
						return Convert.ToInt32(args[idx + 1]);
					}
				}
				return 1;
			}
		}

		/// <summary>
		/// 日志等级(格式：-logLevel 数字(0~6)).
		/// </summary>
		/// <value>日志等级(格式：-logLevel 数字(0~6)).</value>
		public static LogLevel LogLevel {
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return LogLevel.Invalid;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-logLevel") && (args.Length > idx + 1))
					{
						string _value = args [idx + 1];
						if(true == string.IsNullOrEmpty(_value)) {
							return LogLevel.Invalid;
						}
						int _valueTmp = Convert.ToInt32(args[idx + 1]);
						return (LogLevel)_valueTmp;
					}
				}
				return LogLevel.Invalid;
			}
		}

		/// <summary>
		/// Center Version(格式：-centerVersion version).
		/// </summary>
		/// <value>Center Version(格式：-centerVersion version).</value>
		public static string CenterVersion {
			get
			{ 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-centerVersion") && (args.Length > idx + 1))
					{
						return args[idx + 1];
					}
				}
				return null;
			}
		}

		/// <summary>
		///  平台类型.
		/// </summary>
		/// <value>第三方平台SDK类型(格式：-Huawei(华为)).</value>
		public static TPlatformType PlatformType {
			get { 

				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return TPlatformType.None;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if(args[idx] == "-Huawei") {
						return TPlatformType.Huawei;
					} else if (args[idx] == "-Tiange") {
						return TPlatformType.Tiange;
					}
				}
				return TPlatformType.None;
			}
		}

		/// <summary>
		/// BuildNumber(在TeamCity上打包No.格式：-buildNo N).
		/// </summary>
		/// <value>BuildNumber(在TeamCity上打包No.格式：-buildNo N).</value>
		public static int BuildNumber {
			get { 

				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return -1;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if((args[idx] == "-buildNo") && (args.Length > idx + 1))
					{
						string str = args[idx + 1];
						int value;
						if (int.TryParse (str, out value)) {
							return value;
						} else {
							return -1;
						}
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// build Mode(格式：-debug/-release/-store).
		/// </summary>
		/// <value>(格式：-debug/-release/-store).</value>
		public static TBuildMode BuildMode {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return TBuildMode.Debug;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if ("-debug".Equals (args [idx]) == true) {
						return TBuildMode.Debug;
					} else if ("-release".Equals (args [idx]) == true) {
						return TBuildMode.Release;
					} else if ("-store".Equals (args [idx]) == true) {
						return TBuildMode.Store;
					}
				}
				return TBuildMode.None;
			}
		}

		/// <summary>
		/// build Mode(格式：-define [agr2,arg2,arg3,...]).
		/// </summary>
		/// <value>(格式：-define [agr2,arg2,arg3,...]).</value>
		public static string[] Defines {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 2)) {
					return null;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if (("-defines".Equals (args [idx]) == true) && (args.Length >= (idx + 2))) {
						string _temp = args [idx + 1];
						return _temp.Split(',');
					}
				}
				return null;
			}
		}

		/// <summary>
		/// 是否在CI打包编译(格式：-teamCityBuild).
		/// </summary>
		/// <value><c>true</c> CI打包编译; 本地打包编译, <c>false</c>.</value>
		public static bool IsBuildInCI {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return false;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if(args[idx] == "-teamCityBuild")
					{
						return true;
					}
				}
				return false;
			}
		}
			
		/// <summary>
		/// 是否为Cheat模式（可以输入命令行）(格式：-cheat).
		/// </summary>
		/// <value><c>true</c> 可以输入命令行, <c>false</c>不可以输入命令行.</value>
		public static bool IsCheatMode {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return false;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if(args[idx] == "-cheat")
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// 是否为跳过下载（可以输入命令行）(格式：-skipDownload).
		/// </summary>
		/// <value><c>true</c> 跳过下载, <c>false</c>不跳过下载.</value>
		public static bool IsSkipDownload {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();
				if ((args == null) || (args.Length <= 0)) {
					return false;
				}
				for(int idx = 0; idx < args.Length; idx++)
				{
					if(args[idx] == "-skipDownload")
					{
						return true;
					}
				}
				return false;
			}
		}

		private static string _buildTime = null;
		/// <summary>
		/// 打包时间(格式：-buildTime YYYYMMDDHHMMSS).
		/// </summary>
		/// <value>打包时间(格式：-buildTime YYYYMMDDHHMMSS).</value>
		public static string BuildTime {
			get { 
				string[] args = System.Environment.GetCommandLineArgs();

				if ((args != null) && (args.Length > 0)) {
					for (int idx = 0; idx < args.Length; idx++) {
						if (args [idx] == "-buildTime") {
							_buildTime = args [idx + 1];
							break;
						}
					}
				}
				if ((_buildTime == null) || 
					(_buildTime.Equals ("") == true) ||
					(_buildTime.Length == 0)) {
					DateTime nowDateTime = DateTime.Now; 
					_buildTime = string.Format ("{0:yyyyMMddHHmmss}", nowDateTime);
				}
				return _buildTime;
			}
		}

		/// <summary>
		/// 打包日志文件.
		/// </summary>
		/// <value>打包日志文件.</value>
		private static string _buildLogFile = null;
		/// <summary>
		/// 编译打包日志文件(格式：-buildLogFile xxxx.log).
		/// </summary>
		/// <value>编译打包日志文件(格式：-buildLogFile xxxx.log).</value>
		public static string BuildLogFile {
			get {
				if (string.IsNullOrEmpty (_buildLogFile) == false) {
					return _buildLogFile;
				}
				string[] args = System.Environment.GetCommandLineArgs ();
				if ((args != null) && (args.Length > 0)) {
					for (int idx = 0; idx < args.Length; idx++) {
						if (args [idx] == "-buildLogFile") {
							_buildLogFile = args [idx + 1];
						}	
					}
				}
				if (string.IsNullOrEmpty (_buildLogFile) == true) {
					if (true == Application.isMobilePlatform) {
						_buildLogFile = string.Format ("{0}/Logs", 
							Application.persistentDataPath);
					} else {
						_buildLogFile = string.Format ("{0}/Logs", 
							Application.dataPath);
					}
					if (Directory.Exists (_buildLogFile) == false) {
						Directory.CreateDirectory (_buildLogFile);
					}
					_buildLogFile = string.Format ("{0}/{1}.log", 
						_buildLogFile, BuildParameters.BuildTime);
					if (File.Exists (_buildLogFile) == true) {
						File.Delete (_buildLogFile);
					}
				}
				return _buildLogFile;
			}
		}
	}
}