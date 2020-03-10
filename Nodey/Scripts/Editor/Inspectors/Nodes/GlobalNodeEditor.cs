using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace JCMG.Nodey.Editor
{
	[CustomEditor(typeof(Node), true)]
	#if ODIN_INSPECTOR
	public class GlobalNodeEditor : OdinEditor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
			{
				var graphProp = serializedObject.FindProperty("graph");
				var w = NodeEditorWindow.Open(graphProp.objectReferenceValue as NodeGraph);
				w.Home(); // Focus selected node
			}

			base.OnInspectorGUI();
		}
	}
	#else
	public class GlobalNodeEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
			{
				var graphProp = serializedObject.FindProperty("graph");
				var w = NodeEditorWindow.Open(graphProp.objectReferenceValue as NodeGraph);
				w.Home(); // Focus selected node
			}

			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			GUILayout.Label("Raw data", "BoldLabel");

			// Now draw the node itself.
			DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}
	}
	#endif
}
