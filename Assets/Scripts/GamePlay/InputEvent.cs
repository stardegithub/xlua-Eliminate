// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using XLua;
// using Foundation.Databinding.Lua;


// public class InputEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
// {
//     protected Action<Vector3> onPointerDown, onPointerUp;
//     protected LuaTable luaTable;


//     // Use this for initialization
//     void Awake()
//     {
// 		var luaScript = GetComponent<LuaObservableBehaviour>().GetLuaScript();
// 		BindMethod(luaScript);
//     }

//     protected void BindMethod(string luaScript)
//     {
//         luaTable = LuaManager.Instance.LuaEnv.NewTable();
//         LuaTable metaTable = LuaManager.Instance.LuaEnv.NewTable();
//         metaTable.Set("__index", LuaManager.Instance.LuaEnv.Global);
//         luaTable.SetMetaTable(metaTable);
//         metaTable.Dispose();

//         luaTable.Set("self", this.gameObject);


//         luaTable.Get("onPointerDown", out onPointerDown);
//         luaTable.Get("onPointerUp", out onPointerUp);
//     }
//     public void OnPointerDown(PointerEventData eventData)
//     {
// 		onPointerDown(Input.mousePosition);
//     }

//     public void OnPointerUp(PointerEventData eventData)
//     {
//        onPointerUp(Input.mousePosition);
//     }
// }
