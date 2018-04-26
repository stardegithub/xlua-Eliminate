namespace GameSystem
{
    public class SingletonPtr<T> where T : class, new()
    {

        protected static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null) new T();
                return instance;
            }
        }

        protected SingletonPtr()
        {
            instance = this as T;
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void Reset()
        {

        }
    }
}
