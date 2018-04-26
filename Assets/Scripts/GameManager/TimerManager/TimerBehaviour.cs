using System;
using UnityEngine;

namespace GameManager
{
    public class TimerBehaviour : MonoBehaviour, ITimerBehaviour
    {
        private TimerInfo timerInfo;
        public TimerInfo TimerInfo
        {
            get
            {
                return timerInfo;
            }
        }

        private Action onTimerUpdate;

        public void Init(Action onTimerUpdate, string className)
        {
            timerInfo = new TimerInfo(className, this);
            TimerManager.Instance.AddTimerEvent(timerInfo);
            this.onTimerUpdate = onTimerUpdate;
        }

        public void TimerUpdate()
        {
            onTimerUpdate();
        }

        private void OnDestroy()
        {
            if (TimerManager.Instance)
            {
                TimerManager.Instance.RemoveTimerEvent(timerInfo);
            }
        }
    }
}
