using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace GamePlay
{
    public class ObjectPool
    {

        //单例
        //private static ObjectPool instance = null;
        //对象池
        private static Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

        // public static ObjectPool GetInstance()
        // {
        //     if (instance == null)
        //     {
        //         instance = new ObjectPool();
        //         pool = new Dictionary<string, Queue<GameObject>>();
        //     }
        //     return instance;
        // }

        public static void ResetGameObject(GameObject current)
        {
            //设置成非激活状态
            current.SetActive(false);
            //清空父对象
            //		current.transform.parent = null;
            //是否有该类型的对象池
            if (pool.ContainsKey(current.tag))
            {
                //添加到对象池
                pool[current.tag].Enqueue(current);
            }
            else
            {
                pool[current.tag] = new Queue<GameObject>();
                pool[current.tag].Enqueue(current);
            }
        }
        public static GameObject GetGameObject(string objName, Transform parent = null)
        {
            GameObject current;

            //包含此对象池,且有对象
            if (pool.ContainsKey(objName) && pool[objName].Count > 0)
            {
                //获取对象
                current = pool[objName].Dequeue();
            }
            else
            {
                //加载预设体
                GameObject prefab = Resources.Load<GameObject>(objName);
                //生成
                current = GameObject.Instantiate(prefab) as GameObject;

            }
            //设置激活状态
            current.SetActive(true);
            //设置父物体
            current.transform.parent = parent;

            //current.transform.DOScale (Vector3.one, 0.1f);

            //返回
            return current;
        }
    }
}