using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Common;
using GameState;
using EC;

namespace EC.Common
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    /// <typeparam name="PoolManager"></typeparam>
	public class PoolManager : SingletonMonoBehaviourBase<PoolManager>
    {
        private Dictionary<string, IObjectPool> _objectPoolDict;

        #region Singleton
        protected override void SingletonAwake()
        {
            _objectPoolDict = new Dictionary<string, IObjectPool>();
            _initialized = true;
        }

        protected override void SingletonDestroy()
        {
        }
        #endregion

        #region ObjectMap
        /// <summary>
        /// 创建对象表池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>对象表池</returns>
        public ObjectMap<T> CreateObjectMap<T>(string poolName)
        {
            ClearObjectMap<T>(poolName);
            var cashePool = new ObjectMap<T>();
            _objectPoolDict[poolName] = cashePool;
            return cashePool;
        }

        /// <summary>
        /// 是否存在对象表池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ExistsObjectMap<T>(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                return objectPool.GetType() == typeof(ObjectMap<T>);
            }
            return false;
        }

        /// <summary>
        /// 获得对象表池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ObjectMap<T> GetObjectMap<T>(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                if (objectPool.GetType() == typeof(ObjectMap<T>))
                {
                    return objectPool as ObjectMap<T>;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得对象表池元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="elementKey">元素键</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObjectMapElement<T>(string poolName, string elementKey)
        {
            var objectMap = GetObjectMap<T>(poolName);
            if (objectMap != null)
            {
                return objectMap.Get(elementKey);
            }
            return default(T);
        }

        /// <summary>
        /// 推入对象表池元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="elementKey">元素键</param>
        /// <param name="elementValue">元素值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool PushObjectMapElement<T>(string poolName, string elementKey, T elementValue)
        {
            var objectMap = GetObjectMap<T>(poolName);
            if (objectMap != null)
            {
                return objectMap.Push(elementKey, elementValue);
            }
            return false;
        }

        /// <summary>
        /// 创建对象表池并推入元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="elementKey">元素键</param>
        /// <param name="elementValue">元素值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CreateAndPushObjectMapElement<T>(string poolName, string elementKey, T elementValue)
        {
            var objectMap = GetObjectMap<T>(poolName);
            if (objectMap == null)
            {
                objectMap = CreateObjectMap<T>(poolName);
            }
            if (objectMap == null)
            {
                return false;
            }
            return objectMap.Push(elementKey, elementValue);
        }

        /// <summary>
        /// 清理对象表池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        public void ClearObjectMap<T>(string poolName)
        {
            var objectPool = GetObjectMap<T>(poolName);
            if (objectPool != null)
            {
                objectPool.ClearPool();
                _objectPoolDict.Remove(poolName);
            }
        }
        #endregion

        #region ObjectStack
        /// <summary>
        /// 创建对象堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>对象堆池</returns>
        public ObjectStack<T> CreateObjectStack<T>(string poolName)
        {
            ClearObjectStack<T>(poolName);
            var objectStack = new ObjectStack<T>();
            _objectPoolDict[poolName] = objectStack;
            return objectStack;
        }

        /// <summary>
        /// 是否存在对象堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ExistsObjecStack<T>(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                return objectPool.GetType() == typeof(ObjectStack<T>);
            }
            return false;
        }

        /// <summary>
        /// 获得对象堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ObjectStack<T> GetObjectStack<T>(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                if (objectPool.GetType() == typeof(ObjectStack<T>))
                {
                    return objectPool as ObjectStack<T>;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得对象堆池元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObjectStackElement<T>(string poolName)
        {
            var objectStack = GetObjectStack<T>(poolName);
            if (objectStack != null)
            {
                return objectStack.Get();
            }
            return default(T);
        }

        /// <summary>
        /// 推入对象堆池元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="element">元素</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool PushObjectStackElement<T>(string poolName, T element)
        {
            var objectStack = GetObjectStack<T>(poolName);
            if (objectStack != null)
            {
                return objectStack.Push(element);
            }
            return false;
        }

        /// <summary>
        /// 创建对象堆池并推入元素
        /// </summary>
        /// <param name="poolName"池名字></param>
        /// <param name="element">元素</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CreateAndPushObjectStackElement<T>(string poolName, T element)
        {
            var objectStack = GetObjectStack<T>(poolName);
            if (objectStack == null)
            {
                objectStack = CreateObjectStack<T>(poolName);
            }
            if (objectStack == null)
            {
                return false;
            }
            return objectStack.Push(element);
        }


        /// <summary>
        /// 清理对象堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <typeparam name="T"></typeparam>
        public void ClearObjectStack<T>(string poolName)
        {
            var objectStack = GetObjectStack<T>(poolName);
            if (objectStack != null)
            {
                objectStack.ClearPool();
                _objectPoolDict.Remove(poolName);
            }
        }
        #endregion

        #region GameObjectStack
        /// <summary>
        /// 创建游戏对象
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="objectPrefab">预制体</param>
        /// <param name="initCount">初始数量</param>
        /// <param name="maxSize">最大数量</param>
        /// <returns></returns>
        public GameObjectStack CreateGameObjectStack(string poolName, GameObject objectPrefab, int initCount, int maxSize)
        {
            ClearGameObjectStack(poolName);
            Transform poolRoot = new GameObject(poolName).transform;
            poolRoot.SetParent(transform, false);
            var gameObjectStack = new GameObjectStack(poolName, objectPrefab, initCount, maxSize, poolRoot);
            _objectPoolDict[poolName] = gameObjectStack;
            return gameObjectStack;
        }

        /// <summary>
        /// 是否存在游戏对象堆
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <returns></returns>
        public bool ExistsGameObjectStack(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                return objectPool.GetType() == typeof(GameObjectStack);
            }
            return false;
        }

        /// <summary>
        /// 获得游戏对象堆
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <returns></returns>
        public GameObjectStack GetGameObjectStack(string poolName)
        {
            IObjectPool objectPool;
            if (_objectPoolDict.TryGetValue(poolName, out objectPool))
            {
                if (objectPool.GetType() == typeof(GameObjectStack))
                {
                    return objectPool as GameObjectStack;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得游戏对象堆元素
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <returns></returns>
        public GameObject GetGameObjectStackElement(string poolName)
        {
            var gameObjectStack = GetGameObjectStack(poolName);
            if (gameObjectStack != null)
            {
                return gameObjectStack.Get();
            }
            return null;
        }

        /// <summary>
        /// 推入游戏对象堆
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="element">游戏对象</param>
        /// <returns></returns>
        public bool PushGameObjectsStackElement(string poolName, GameObject element)
        {
            var gameObjectStack = GetGameObjectStack(poolName);
            if (gameObjectStack != null)
            {
                return gameObjectStack.Push(element);
            }
            return false;
        }

        /// <summary>
        /// 延迟推入游戏对象堆
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="element">游戏对象</param>
        /// <param name="delay">延迟时间</param>
        public void PushGameObjectStackElement(string poolName, GameObject element, float delay)
        {
            StartCoroutine(PushGameObjectStackElement_Routine(poolName, element, delay));
        }

        private IEnumerator PushGameObjectStackElement_Routine(string poolName, GameObject element, float delay)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
            PushGameObjectsStackElement(poolName, element);
        }

        /// <summary>
        /// 清理游戏对象堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        public void ClearGameObjectStack(string poolName)
        {
            var gameObjectStack = GetGameObjectStack(poolName);
            if (gameObjectStack != null)
            {
                gameObjectStack.ClearPool();
                _objectPoolDict.Remove(poolName);
            }
        }
        #endregion

        /// <summary>
        /// 是否存在对象池
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public bool ExistsObjectPool(string poolName)
        {
            return _objectPoolDict.ContainsKey(poolName);
        }

        /// <summary>
        /// 清理所以池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var element in _objectPoolDict)
            {
                element.Value.ClearPool();
            }
            _objectPoolDict.Clear();
        }
    }

    /// <summary>
    /// 对象池接口
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// 清理池
        /// </summary>
        void ClearPool();
    }

    /// <summary>
    /// 对象表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectMap<T> : IObjectPool
    {
        protected Dictionary<string, T> casheMap;

        public ObjectMap()
        {
            casheMap = new Dictionary<string, T>();
        }

        /// <summary>
        /// 获得元素
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public virtual T Get(string key)
        {
            if (casheMap.ContainsKey(key))
            {
                return casheMap[key];
            }
            return default(T);
        }

        /// <summary>
        /// 推入元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public virtual bool Push(string key, T value)
        {
            if (value == null) return false;
            casheMap[key] = value;
            return true;
        }

        /// <summary>
        /// 清理池
        /// </summary>
        public virtual void ClearPool()
        {
            foreach (var element in casheMap)
            {
                if (element.GetType().IsAssignableFrom(typeof(Object)))
                {
                    Resources.UnloadAsset(element.Value as Object);
                }
            }

            casheMap.Clear();
        }
    }

    /// <summary>
    /// 对象堆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectStack<T> : IObjectPool
    {
        protected Stack<T> objectStack;

        public ObjectStack()
        {
            objectStack = new Stack<T>();
        }

        /// <summary>
        /// 获得元素
        /// </summary>
        /// <returns></returns>
        public virtual T Get()
        {
            if (objectStack.Count > 0)
            {
                return objectStack.Pop();
            }
            return default(T);
        }

        /// <summary>
        /// 推入元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public virtual bool Push(T element)
        {
            objectStack.Push(element);
            return true;
        }

        /// <summary>
        /// 清理池
        /// </summary>
        public virtual void ClearPool()
        {
            objectStack.Clear();
        }
    }

    /// <summary>
    /// 游戏对象堆
    /// </summary>
    /// <typeparam name="GameObject"></typeparam>
    public class GameObjectStack : ObjectStack<GameObject>
    {
        private int maxSize;
        // private int poolSize;
        private Transform poolRoot;
        private GameObject objectPrefab;

        /// <summary>
        /// 缓存父物体
        /// </summary>
        /// <returns></returns>
        public Transform PoolRoot { get { return poolRoot; } }

        /// <summary>
        /// 构造游戏堆池
        /// </summary>
        /// <param name="poolName">池名字</param>
        /// <param name="objectPrefab">预制体</param>
        /// <param name="initCount">初始数量</param>
        /// <param name="maxSize">最大数量</param>
        /// <param name="poolRoot">缓存父物体</param>
        public GameObjectStack(string poolName, GameObject objectPrefab, int initCount, int maxSize, Transform poolRoot)
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

        /// <summary>
        /// 获得元素
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 推入元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 清理池
        /// </summary>
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
