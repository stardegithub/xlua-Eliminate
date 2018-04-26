using System.Collections.Generic;
using System.Threading;
using System;

namespace GameManager
{
    public class ThreadPoolManager : GameManagerBase<ThreadPoolManager>
    {
        private int locker;
        private int count;
        private Queue<ParametricAction> TheActions = new Queue<ParametricAction>(13);

        public int GetCount()
        {
            return count;
        }

        #region Singleton
        protected override void SingletonAwake()
        {
            initialized = true;
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            ParametricAction? nowAction = null;

            if (count > 0 && Interlocked.CompareExchange(ref locker, 1, 0) == 0)  //能够从0换成1
            {
                try
                {
                    if (count > 0)
                    {
                        nowAction = TheActions.Dequeue();
                        count--;

                    }
                }
                finally
                {
                    locker = 0;
                }
            }

            if (nowAction != null)
            {
                nowAction.Value.TheAction(nowAction.Value.Parameter);
            }
        }

        public void QueueOnU3DThread(object obj, Action<object> action)
        {
        Redo:
            if (Interlocked.CompareExchange(ref locker, 1, 0) == 0)  //能够从0换成1
            {
                TheActions.Enqueue(new ParametricAction(action, obj));
                count++;
                locker = 0;
            }
            else  //CPU自旋等待
            {
                goto Redo;
            }
        }

        public void QueueOnOtherThread(object obj, Action<object> action)
        {

#if (UNITY_WINRT_8_0 || UNITY_WINRT_8_1) && !UNITY_EDITOR
        Windows.System.Threading.ThreadPool.RunAsync(new Windows.System.Threading.WorkItemHandler((ia) =>
#else
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((o) =>
#endif
            {
                action(obj);
            }));
        }
    }

    public struct ParametricAction
    {
        public Action<object> TheAction;

        public object Parameter;

        public ParametricAction(Action<object> action, object parameter)
        {
            TheAction = action;
            Parameter = parameter;
        }
    }
}
