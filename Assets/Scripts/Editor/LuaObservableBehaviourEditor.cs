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
            var newTxtAsset = EditorGUILayout.ObjectField("LuaText", lob._luaText, typeof(TextAsset), true) as TextAsset;
            if (newTxtAsset != lob._luaText)
            {
                lob._luaText = newTxtAsset;
                lob._luaFilePath = AssetDatabase.GetAssetPath(newTxtAsset);
            }
        }
    }
}
