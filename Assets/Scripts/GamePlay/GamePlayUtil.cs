using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using DG.Tweening;

namespace GamePlay
{
    public class GamePlayUtil
    {

        //目录
        public const string ResourcesPrefab = "Prefabs/GamePlay";
        //文件名称
        public const string BlockPath = "Prefab/GamePlay/Block";
        public const string BlockCanvasPath = "Prefab/GamePlay/BlockCanvas";

        //动画参数名称
        public const string Pressed = "Pressed";
        public const string Exit = "Exit";

        //参数
        public static float BlockMoveTime = 0.2f;
        public static float BlockDropTime = 0.2f;

        public static Dictionary<EBlockType, Sprite> randomSprites = new Dictionary<EBlockType, Sprite>();
        public static Sprite GetSpriteAssetsByType(EBlockType type)
        {
            Sprite sprite = null;
            if (!randomSprites.ContainsKey(type))
            {
                switch (type)
                {
                    case EBlockType.Apple:
                        sprite = Resources.Load<Sprite>("Texture/Eliminate/1");
                        break;
                    case EBlockType.Banana:
                        sprite = Resources.Load<Sprite>("Texture/Eliminate/2");
                        break;
                    case EBlockType.Grape:
                        sprite = Resources.Load<Sprite>("Texture/Eliminate/3");
                        break;
                    case EBlockType.Lemon:
                        sprite = Resources.Load<Sprite>("Texture/Eliminate/4");
                        break;
                    case EBlockType.Pear:
                        sprite = Resources.Load<Sprite>("Texture/Eliminate/5");
                        break;
                }
                randomSprites[type] = sprite;
            }

            return randomSprites[type];
        }

        public static Transform InstantiateGameObject(string path)
        {
            Debug.Log(path);
            var go = Resources.Load<GameObject>(path);
                        Debug.Log(go);

            return GameObject.Instantiate(go).transform;
        }

        public static int Range(int start,int end)
        {
            if(start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }
            int random = Random.Range(start, end);
            return random;
        }
        //block类型
        [LuaCallCSharp]
        public enum EBlockType
        {
            //None = -1,
            //普通可消除类型
            Apple = 0,
            Banana,
            Pear,
            Grape,
            Lemon,
            Num,
        }
        public static int EBlockTypeToInt(EBlockType type)
        {
            return (int)type;
        }

        [LuaCallCSharp]
        public enum EBlockAttribute
        {
            None = 0,
            Ice,

        }
        [LuaCallCSharp]
        public enum EEliminateType
        {
            Default = -1,
            Ttype = 0,
            Ltype,
            Itype,
            Xtype,
        }
		

    }
}