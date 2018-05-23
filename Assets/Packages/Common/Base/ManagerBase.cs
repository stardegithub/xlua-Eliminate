using System;

namespace Common
{
	/// <summary>
	/// 管理器（基类）.
	/// </summary>
    public interface IManagerBase
    {
        bool Initialized { get; }

        void OnBeginStateEnter(string currStateName, string nextStateName);
        void OnEndStateExit(string currStateName, string nextStateName);
        void OnMemoryWarning(float currMem, float devMem, float perc);
    }

    public class GameManageHelper
    {
		public static IManagerBase GetCustomManage(Type type)
        {
            // if (type == typeof(ThreadPoolManager))
            // {
            //     return ThreadPoolManager.Instance;
            // }
            return null;
        }
    }
}