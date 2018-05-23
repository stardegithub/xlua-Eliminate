using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Common;

namespace AutoResize {

	/// <summary>
	/// UI自动适配控制器的编辑器.
	/// </summary>
	[CustomEditor(typeof(UIAutoResizeController))]  
	public class UIAutoResizeControllerEditor : EditorBase {

#region Inspector

		public override void OnInspectorGUI ()  {
			serializedObject.Update ();

			UIAutoResizeController _target = target as UIAutoResizeController; 
			if (null != _target) {

				if (null == _target.Padding) {
					_target.Padding = new RectOffset (0, 0, 0, 0);
				}

#if BUILD_DEBUG
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("IsDebugTest", 
					GUILayout.Width(75.0f), GUILayout.Height(20.0f));
				_target.IsDebugTest = EditorGUILayout.Toggle(_target.IsDebugTest, 
					GUILayout.Width(35.0f), GUILayout.Height(20.0f));
				EditorGUILayout.EndHorizontal ();
#endif

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Padding", 
					GUILayout.Width(105.0f), GUILayout.Height(20.0f));
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (" ", 
					GUILayout.Width(10.0f), GUILayout.Height(20.0f));
				{
					EditorGUILayout.LabelField ("Left", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.Padding.left = EditorGUILayout.IntField (_target.Padding.left, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				{
					EditorGUILayout.LabelField ("Right", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.Padding.right = EditorGUILayout.IntField (_target.Padding.right, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (" ", 
					GUILayout.Width(10.0f), GUILayout.Height(20.0f));
				{
					EditorGUILayout.LabelField ("Top", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.Padding.top = EditorGUILayout.IntField (_target.Padding.top, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				{
					EditorGUILayout.LabelField ("Bottom", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.Padding.bottom = EditorGUILayout.IntField (_target.Padding.bottom, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("PosOffset", 
					GUILayout.Width(105.0f), GUILayout.Height(20.0f));
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (" ", 
					GUILayout.Width(10.0f), GUILayout.Height(20.0f));
				{
					EditorGUILayout.LabelField ("X", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.PosOffset.x = EditorGUILayout.FloatField (_target.PosOffset.x, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				{
					EditorGUILayout.LabelField ("Y", 
						GUILayout.Width(45.0f), GUILayout.Height(20.0f));
					_target.PosOffset.y = EditorGUILayout.FloatField (_target.PosOffset.y, 
						GUILayout.Width(60.0f), GUILayout.Height(20.0f));
				}
				EditorGUILayout.EndHorizontal ();

				if (GUILayout.Button ("Apply", 
					GUILayout.Width(240.0f), GUILayout.Height(20.0f))) {
					_target.Apply ();
				}

			}

			// 保存变化后的值
			serializedObject.ApplyModifiedProperties();
		}

#endregion
	}
}
