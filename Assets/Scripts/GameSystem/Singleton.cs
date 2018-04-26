using System;
using UnityEngine;

namespace GameSystem
{
    public interface ISingleton
    {
    }

    public abstract class Singleton<T> : MonoBehaviour, ISingleton where T : Singleton<T>
    {
        private static T instance;
        public static T Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null)
            {
                // AppLogger.Error("duplicate singleton:{0}, current:{1}, new:{2}, destroy new", typeof(T), _instance.transform.GetInstanceID(), transform.GetInstanceID());
                Destroy(this);
                return;
            }
            instance = this as T;
            SingletonAwake();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SingletonDestroy();
                instance = null;
            }
        }

        protected virtual void SingletonAwake() { }
        protected virtual void SingletonDestroy() { }
    }
}
