using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSystem;

namespace GameManager
{
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
        /// ������ʱ��
        /// </summary>
        /// <param name="interval"></param>
        public void StartTimer(float value)
        {
            interval = value;
            InvokeRepeating("Run", 0, interval);
        }

        /// <summary>
        /// ֹͣ��ʱ��
        /// </summary>
        public void StopTimer()
        {
            CancelInvoke("Run");
        }

        /// <summary>
        /// ���Ӽ�ʱ���¼�
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
        /// ɾ����ʱ���¼�
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.delete = true;
            }
        }

        /// <summary>
        /// ֹͣ��ʱ���¼�
        /// </summary>
        /// <param name="info"></param>
        public void StopTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.stop = true;
            }
        }

        /// <summary>
        /// ������ʱ���¼�
        /// </summary>
        /// <param name="info"></param>
        public void ResumeTimerEvent(TimerInfo info)
        {
            if (objects.Contains(info) && info != null)
            {
                info.delete = false;
            }
        }

        /// <summary>
        /// ��ʱ������
        /// </summary>
        void Run()
        {
            if (objects.Count == 0) return;
            for (int i = 0; i < objects.Count; i++)
            {
                TimerInfo o = objects[i];
                if (o.delete || o.stop) { continue; }
                o.target.TimerUpdate();
                o.tick++;
            }
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].delete) { objects.Remove(objects[i]); }
            }
        }
    }
}