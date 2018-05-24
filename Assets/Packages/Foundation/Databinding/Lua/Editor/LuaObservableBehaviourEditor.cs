using UnityEditor;
using UnityEngine;
using Common;

namespace Foundation.Databinding.Lua
{
    [CustomEditor(typeof(LuaObservableBehaviour))]
	public class LuaObservableBehaviourEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var lob = target as LuaObservableBehaviour;
            var newLuaText = EditorGUILayout.ObjectField("LuaText", lob.LuaText, typeof(TextAsset), true) as TextAsset;
            if (newLuaText != lob.LuaText)
            {
                lob.LuaText = newLuaText;
                lob.LuaTextPath = AssetDatabase.GetAssetPath(newLuaText);
            }

            var newLuaTextPath = EditorGUILayout.TextField("LuaTextPath", lob.LuaTextPath);
            if (newLuaTextPath != lob.LuaTextPath)
            {
                lob.LuaText = AssetDatabase.LoadAssetAtPath<TextAsset>(newLuaTextPath);
                lob.LuaTextPath = newLuaTextPath;
            }
        }
    }
}
