using UnityEngine;
using Common;

namespace BuildSystem.BuildLoggerImpl
{
	public class ConsoleBuildLogger : ClassExtension, IBuildLogger
	{
		#region IBuildLogger implementation
		
		void IBuildLogger.OpenBlock(string blockName)
		{
			if(!string.IsNullOrEmpty(blockName))
			{
				var message = string.Format("==STARTING {0}==", blockName.Trim()).ToUpper();
				Log(message);
			}
		}
		
		void IBuildLogger.CloseBlock(string blockName)
		{
			if(!string.IsNullOrEmpty(blockName))
			{
				var message = string.Format("=={0} FINISHED==", blockName.Trim()).ToUpper();
				Log(message);
			}
		}
		
		void IBuildLogger.LogMessage(string message)
		{
			Log(message);
		}
		
		void IBuildLogger.LogWarning(string message)
		{
			message = string.Format("warning: {0}", message);
			Log(message, TLogType.kWarning);
		}

		void IBuildLogger.LogError(string message)
		{
			message = string.Format("error: {0}", message);
			Log(message, TLogType.kError);
		}

		void IBuildLogger.LogException(string message)
		{
			message = string.Format("exception: {0}", message);
			Log(message, TLogType.kException);
		}

		#endregion

		private void Log(string message, TLogType iType = TLogType.kInfo) {
			System.Console.WriteLine(message);
			if (BuildParameters.IsBuildInCI == false) {
				switch (iType) {
				case TLogType.kInfo:
					{
						this.Info ("Log()::{0}", message);
					}
					break;
				case TLogType.kWarning:
					{
						this.Warning ("Log()::{0}", message);
					}
					break;
				case TLogType.kError:
					{
						this.Error ("Log()::{0}", message);
					}
					break;
				case TLogType.kException:
					{
						this.Fatal ("Log()::{0}", message);
					}
					break;
				default:
					break;
				}
			}
		}

	}
} //namespace UnityBuildSystem.BuildLoggerImpl
