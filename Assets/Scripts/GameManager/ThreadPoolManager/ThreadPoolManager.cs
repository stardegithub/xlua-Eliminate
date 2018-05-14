using System.Collections.Generic;
using System.Threading;
using System;

namespace GameManager
{
    /// <summary>
    /// 线程池管理器
    /// </summary>
    /// <typeparam name="ThreadPoolManager"></typeparam>
    public class ThreadPoolManager : GameManagerBase<ThreadPoolManager>
    {
        private int _locker;
        private int _count;
        /// <summary>
        /// 主线程处理函数队列
        /// </summary>
        /// <typeparam name="ParametricAction"></typeparam>
        /// <returns></returns>
        private Queue<ParametricAction> TheActions = new Queue<ParametricAction>(13);

        /// <summary>
        /// 主线程处理函数数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _count;
        }

        #region Singleton
        protected override void SingletonAwake()
        {
            _initialized = true;
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            ParametricAction? nowAction = null;

            if (_count > 0 && Interlocked.CompareExchange(ref _locker, 1, 0) == 0)  //能够从0换成1
            {
                try
                {
                    if (_count > 0)
                    {
                        nowAction = TheActions.Dequeue();
                        _count--;

                    }
                }
                finally
                {
                    _locker = 0;
                }
            }

            if (nowAction != null)
            {
                nowAction.Value.TheAction(nowAction.Value.Parameter);
            }
        }

        /// <summary>
        /// 其它线程函数交给主线程处理
        /// </summary>
        /// <param name="obj">函数参数</param>
        /// <param name="action">函数</param>
        public void QueueOnU3DThread(object obj, Action<object> action)
        {
        Redo:
            if (Interlocked.CompareExchange(ref _locker, 1, 0) == 0)  //能够从0换成1
            {
                TheActions.Enqueue(new ParametricAction(action, obj));
                _count++;
                _locker = 0;
            }
            else  //CPU自旋等待
            {
                goto Redo;
            }
        }

        /// <summary>
        /// 交给其它空闲线程处理函数
        /// </summary>
        /// <param name="obj">函数参数</param>
        /// <param name="action">函数</param>
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
