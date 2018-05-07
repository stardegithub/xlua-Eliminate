using GameSystem;
using UnityEditor;
using UnityEngine;

namespace GameEditor
{
    [CustomEditor(typeof(LuaObservableBehaviour))]
    public class LuaObservableBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var lob = target as LuaObservableBehaviour;
            var newLuaText = EditorGUILayout.ObjectField("LuaText", lob._luaText, typeof(TextAsset), true) as TextAsset;
            if (newLuaText != lob._luaText)
            {
                lob._luaText = newLuaText;
                lob._luaTextPath = AssetDatabase.GetAssetPath(newLuaText);
            }

            var newLuaTextPath = EditorGUILayout.TextField("LuaTextPath", lob._luaTextPath);
            if (newLuaTextPath != lob._luaTextPath)
            {
                lob._luaText = AssetDatabase.LoadAssetAtPath<TextAsset>(newLuaTextPath);
                lob._luaTextPath = newLuaTextPath;
            }
        }
    }
}
