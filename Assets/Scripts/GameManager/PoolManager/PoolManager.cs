using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace GameManager
{
    public class PoolManager : GameManagerBase<PoolManager>
    {
        private Dictionary<string, ICashePool> cashePoolMap;
        private Dictionary<string, IObjectPool> objectPoolMap;

        #region Singleton
        protected override void SingletonAwake()
        {
            cashePoolMap = new Dictionary<string, ICashePool>();
            objectPoolMap = new Dictionary<string, IObjectPool>();
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
        }
        #endregion

        #region CashePool
        public CashePool<T> CreateCashePool<T>(string poolName) where T : Object
        {
            ClearCashePool<T>(poolName);
            var cashePool = new CashePool<T>();
            cashePoolMap[poolName] = cashePool;
            return cashePool;
        }

        public bool ExistsCashePool<T>(string poolName) where T : Object
        {
            ICashePool cashePool;
            if (cashePoolMap.TryGetValue(poolName, out cashePool))
            {
                return cashePool.GetType() == typeof(CashePool<T>);
            }
            return false;
        }

        public CashePool<T> GetCashePool<T>(string poolName) where T : Object
        {
            ICashePool cashePool;
            if (cashePoolMap.TryGetValue(poolName, out cashePool))
            {
                if (cashePool.GetType() == typeof(CashePool<T>))
                {
                    return cashePool as CashePool<T>;
                }
            }
            return null;
        }

        public T GetCasheElement<T>(string poolName, string elementName) where T : Object
        {
            var cashePool = GetCashePool<T>(poolName);
            if (cashePool != null)
            {
                return cashePool.Get(elementName);
            }
            return null;
        }

        public bool PushCasheElement<T>(string poolName, T element) where T : Object
        {
            var cashePool = GetCashePool<T>(poolName);
            if (cashePool != null)
            {
                return cashePool.Push(element);
            }
            return false;
        }

        public void ClearCashePool<T>(string poolName) where T : Object
        {
            var cashePool = GetCashePool<T>(poolName);
            if (cashePool != null)
            {
                cashePool.ClearPool();
                cashePoolMap.Remove(poolName);
            }
        }
        #endregion

        #region ObjectPool
        public ObjectPool<T> CreateObjectPool<T>(string poolName)
        {
            ClearObjectPool<T>(poolName);
            var objectPool = new ObjectPool<T>();
            objectPoolMap[poolName] = objectPool;
            return objectPool;
        }

        public bool ExistsObjecPool<T>(string poolName)
        {
            IObjectPool objectPool;
            if (objectPoolMap.TryGetValue(poolName, out objectPool))
            {
                return objectPool.GetType() == typeof(ObjectPool<T>);
            }
            return false;
        }

        public ObjectPool<T> GetObjectPool<T>(string poolName)
        {
            IObjectPool objectPool;
            if (objectPoolMap.TryGetValue(poolName, out objectPool))
            {
                if (objectPool.GetType() == typeof(ObjectPool<T>))
                {
                    return objectPool as ObjectPool<T>;
                }
            }
            return null;
        }

        public T GetObjectElement<T>(string poolName)
        {
            var objectPool = GetObjectPool<T>(poolName);
            if (objectPool != null)
            {
                return objectPool.Get();
            }
            return default(T);
        }

        public bool PushObjectElement<T>(string poolName, T element)
        {
            var objectPool = GetObjectPool<T>(poolName);
            if (objectPool != null)
            {
                return objectPool.Push(element);
            }
            return false;
        }

        public void ClearObjectPool<T>(string poolName)
        {
            var objectPool = GetObjectPool<T>(poolName);
            if (objectPool != null)
            {
                objectPool.ClearPool();
                objectPoolMap.Remove(poolName);
            }
        }
        #endregion

        #region GameObjectPool
        public GameObjectPool CreateGameObjectPool(string poolName, GameObject objectPrefab, int initCount, int maxSize)
        {
            ClearGameObjectPool(poolName);
            Transform poolRoot = new GameObject(poolName).transform;
            poolRoot.SetParent(transform, false);
            var gameObjectPool = new GameObjectPool(poolName, objectPrefab, initCount, maxSize, poolRoot);
            objectPoolMap[poolName] = gameObjectPool;
            return gameObjectPool;
        }

        public bool ExistsGameObjecPool(string poolName)
        {
            IObjectPool objectPool;
            if (objectPoolMap.TryGetValue(poolName, out objectPool))
            {
                return objectPool.GetType() == typeof(GameObjectPool);
            }
            return false;
        }

        public GameObjectPool GetGameObjectPool(string poolName)
        {
            IObjectPool objectPool;
            if (objectPoolMap.TryGetValue(poolName, out objectPool))
            {
                if (objectPool.GetType() == typeof(GameObjectPool))
                {
                    return objectPool as GameObjectPool;
                }
            }
            return null;
        }

        public GameObject GetGameObjectElement(string poolName)
        {
            var gameObjectPool = GetGameObjectPool(poolName);
            if (gameObjectPool != null)
            {
                return gameObjectPool.Get();
            }
            return null;
        }

        public bool PushGameObjectElement(string poolName, GameObject element)
        {
            var gameObjectPool = GetGameObjectPool(poolName);
            if (gameObjectPool != null)
            {
                return gameObjectPool.Push(element);
            }
            return false;
        }

        public void PushGameObjectElement(string poolName, GameObject element, float delay)
        {
            StartCoroutine(PushGameObjectElement_Routine(poolName, element, delay));
        }

        private IEnumerator PushGameObjectElement_Routine(string poolName, GameObject element, float delay)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
            PushGameObjectElement(poolName, element);
        }

        public void ClearGameObjectPool(string poolName)
        {
            var gameObjectPool = GetGameObjectPool(poolName);
            if (gameObjectPool != null)
            {
                gameObjectPool.ClearPool();
                objectPoolMap.Remove(poolName);
            }
        }
        #endregion

        public void ClearAllPools()
        {
            foreach (var element in cashePoolMap)
            {
                element.Value.ClearPool();
            }
            cashePoolMap.Clear();

            foreach (var element in objectPoolMap)
            {
                element.Value.ClearPool();
            }
            objectPoolMap.Clear();
        }
    }

    public interface ICashePool
    {
        void ClearPool();
    }

    public class CashePool<T> : ICashePool where T : Object
    {
        protected bool unLoadOnClear;
        protected Dictionary<string, T> casheMap;

        public CashePool()
        {
            unLoadOnClear = true;
            casheMap = new Dictionary<string, T>();
        }

        public virtual T Get(string name)
        {
            if (casheMap.ContainsKey(name))
            {
                return casheMap[name];
            }
            return null;
        }

        public virtual bool Push(T element)
        {
            if (element == null) return false;
            casheMap[element.name] = element;
            return true;
        }

        public virtual void ClearPool()
        {
            if (unLoadOnClear)
            {
                foreach (var element in casheMap)
                {
                    Resources.UnloadAsset(element.Value);
                }
            }

            casheMap.Clear();
        }
    }

    public interface IObjectPool
    {
        void ClearPool();
    }

    public class ObjectPool<T> : IObjectPool
    {
        protected Stack<T> objectStack;

        public ObjectPool()
        {
            objectStack = new Stack<T>();
        }

        public virtual T Get()
        {
            if (objectStack.Count > 0)
            {
                return objectStack.Pop();
            }
            return default(T);
        }

        public virtual bool Push(T element)
        {
            objectStack.Push(element);
            return true;
        }

        public virtual void ClearPool()
        {
        }
    }

    public class GameObjectPool : ObjectPool<GameObject>
    {
        private int maxSize;
        // private int poolSize;
        private Transform poolRoot;
        private GameObject objectPrefab;

        public Transform PoolRoot { get { return poolRoot; } }

        public GameObjectPool(string poolName, GameObject objectPrefab, int initCount, int maxSize, Transform poolRoot)
        {
            this.maxSize = maxSize;
            // this.poolSize = initCount;
            this.poolRoot = poolRoot;
            this.objectPrefab = objectPrefab;

            //populate the pool
            for (int index = 0; index < initCount; index++)
            {
                AddObjectToPool(CreateInstance());
            }
        }

        private void AddObjectToPool(GameObject go)
        {
            //add to pool
            go.SetActive(false);
            objectStack.Push(go);
            go.transform.SetParent(poolRoot, true);
        }

        private GameObject CreateInstance()
        {
            return GameObject.Instantiate(objectPrefab) as GameObject;
        }

        public override GameObject Get()
        {
            if (objectStack.Count == 0)
            {
                AddObjectToPool(CreateInstance());
            }
            GameObject go = objectStack.Pop();
            go.SetActive(true);
            return go;
        }

        public override bool Push(GameObject element)
        {
            if (!element)
            {
                return false;
            }

            if (maxSize <= 0 || objectStack.Count < maxSize)
            {
                AddObjectToPool(element);
                return true;
            }
            else
            {
                element.SetActive(false);
                GameObject.Destroy(element);
                return false;
            }
        }

        public override void ClearPool()
        {
            // while (objectStack.Count > 0)
            // {
            //     GameObject.Destroy(objectStack.Pop());
            // }
            Object.Destroy(poolRoot.gameObject);
            objectStack = null;
            objectPrefab = null;
        }
    }
}
