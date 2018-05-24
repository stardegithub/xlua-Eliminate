using UnityEditor;
using UnityEngine;
using Common;

namespace Facade.Lua
{
    [CustomEditor(typeof(FacadeLuaObserver))]
	public class FacadeLuaObserverEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var flo = target as FacadeLuaObserver;
            var newLuaText = EditorGUILayout.ObjectField("LuaText", flo.LuaText, typeof(TextAsset), true) as TextAsset;
            if (newLuaText != flo.LuaText)
            {
                flo.LuaText = newLuaText;
                flo.LuaTextPath = AssetDatabase.GetAssetPath(newLuaText);
            }

            var newLuaTextPath = EditorGUILayout.TextField("LuaTextPath", flo.LuaTextPath);
            if (newLuaTextPath != flo.LuaTextPath)
            {
                flo.LuaText = AssetDatabase.LoadAssetAtPath<TextAsset>(newLuaTextPath);
                flo.LuaTextPath = newLuaTextPath;
            }
        }
    }
}
