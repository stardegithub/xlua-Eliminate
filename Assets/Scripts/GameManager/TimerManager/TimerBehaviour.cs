using System;
using UnityEngine;

namespace GameManager
{
    public class TimerBehaviour : MonoBehaviour, ITimerBehaviour
    {
        private TimerInfo _timerInfo;
        /// <summary>
        /// 计时信息
        /// </summary>
        /// <returns></returns>
        public TimerInfo TimerInfo
        {
            get
            {
                return _timerInfo;
            }
        }

        private Action _onTimerUpdate;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onTimerUpdate"></param>
        /// <param name="className"></param>
        public void Init(Action onTimerUpdate, string className)
        {
            _timerInfo = new TimerInfo(className, this);
            TimerManager.Instance.AddTimerEvent(_timerInfo);
            this._onTimerUpdate = onTimerUpdate;
        }

        public void TimerUpdate()
        {
            _onTimerUpdate();
        }

        private void OnDestroy()
        {
            if (TimerManager.Instance)
            {
                TimerManager.Instance.RemoveTimerEvent(_timerInfo);
            }
        }
    }
}
