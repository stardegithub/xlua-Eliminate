using System;
using Common;

namespace GameState
{
    public interface IGameManager
    {
        bool Initialized { get; }

        void OnBeginStateEnter(string currStateName, string nextStateName);
        void OnEndStateExit(string currStateName, string nextStateName);
        void OnMemoryWarning(float currMem, float devMem, float perc);
    }

    public abstract class GameManagerBase<T> : SingletonMonoBehaviourBase<T>, IGameManager where T : GameManagerBase<T>
    {
        protected bool _initialized;

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public virtual void OnBeginStateEnter(string currStateName, string nextStateName)
        {
        }

        public virtual void OnEndStateExit(string currStateName, string nextStateName)
        {
        }

        public virtual void OnMemoryWarning(float currMem, float devMem, float perc)
        {
        }
    }

    public abstract class GameManagerClassBase<T> : IGameManager where T : GameManagerClassBase<T>
    {
        private bool initialized;

        public bool Initialized
        {
            get
            {
                return Initialized;
            }
        }

        public virtual void OnBeginStateEnter(string currStateName, string nextStateName)
        {
        }

        public virtual void OnEndStateExit(string currStateName, string nextStateName)
        {
        }

        public virtual void OnMemoryWarning(float currMem, float devMem, float perc)
        {
        }
    }

    public class GameManageHelper
    {
        public static IGameManager GetCustomManage(Type type)
        {
            // if (type == typeof(ThreadPoolManager))
            // {
            //     return ThreadPoolManager.Instance;
            // }
            return null;
        }
    }
}
