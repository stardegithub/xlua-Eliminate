using System;
using System.Collections;

namespace GameManager {
    public interface ITimerBehaviour {
        void TimerUpdate();
    }

    public class TimerInfo
    {
        public long tick;
        public bool stop;
        public bool delete;
        public ITimerBehaviour target;
        public string className;

        public TimerInfo(string className, ITimerBehaviour target)
        {
            this.className = className;
            this.target = target;
            delete = false;
        }
    }
}