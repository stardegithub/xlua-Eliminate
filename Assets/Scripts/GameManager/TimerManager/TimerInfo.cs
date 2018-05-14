using System;
using System.Collections;

namespace GameManager {
    public interface ITimerBehaviour {
        void TimerUpdate();
    }

    /// <summary>
    /// 计时信息
    /// </summary>
    public class TimerInfo
    {
        public long Tick;
        public bool Stop;
        public bool Delete;
        public ITimerBehaviour Target;
        public string ClassName;

        public TimerInfo(string className, ITimerBehaviour target)
        {
            this.ClassName = className;
            this.Target = target;
            Delete = false;
        }
    }
}