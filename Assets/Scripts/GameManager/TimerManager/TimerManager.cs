using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSystem;

namespace GameManager
{
    /// <summary>
    /// 计时器管理器
    /// </summary>
    /// <typeparam name="TimerManager"></typeparam>
    public class TimerManager : GameManagerBase<TimerManager>
    {
        private float interval = 0;
        private List<TimerInfo> objects = new List<TimerInfo>();

        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        // Use this for initialization
        void Start()
        {
            StartTimer(GameConfig.Instance.timerInterval);
        }

        /// <summary>
        /// 启动计时器
        /// </summary>
        /// <param name="interval"></param>
        public void StartTimer(float value)
        {
            interval = value;
            InvokeRepeating("Run", 0, interval);
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void StopTimer()
        {
            CancelInvoke("Run");
        }

        /// <summary>
        /// 添加计时器事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        public void AddTimerEvent(TimerInfo info)
        {
            if (!objects.Contains(info))
            {
                objects.Add(info);
            }
        }

        /// <summary>
        /// 删除计时器事件
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.Delete = true;
            }
        }

        /// <summary>
        /// 停止计时器事件
        /// </summary>
        /// <param name="info"></param>
        public void StopTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.Stop = true;
            }
        }

        /// <summary>
        /// 继续计时器事件
        /// </summary>
        /// <param name="info"></param>
        public void ResumeTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.Delete = false;
            }
        }

        /// <summary>
        /// 计时器运行
        /// </summary>
        void Run()
        {
            if (objects.Count == 0) return;
            for (int i = 0; i < objects.Count; i++)
            {
                TimerInfo o = objects[i];
                if (o.Delete || o.Stop) { continue; }
                o.Target.TimerUpdate();
                o.Tick++;
            }
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].Delete) { objects.Remove(objects[i]); }
            }
        }
    }
}