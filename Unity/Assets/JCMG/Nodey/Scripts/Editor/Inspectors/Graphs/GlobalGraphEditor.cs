using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace JCMG.Nodey.Editor
{
	/// <summary> Override graph inspector to show an 'Open Graph' button at the top </summary>
	[CustomEditor(typeof(NodeGraph), true)]
	#if ODIN_INSPECTOR
	public class GlobalGraphEditor : OdinEditor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
			{
				NodeEditorWindow.Open(serializedObject.targetObject as NodeGraph);
			}

			base.OnInspectorGUI();
		}
	}
	#else
	public class GlobalGraphEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
			{
				NodeEditorWindow.Open(serializedObject.targetObject as NodeGraph);
			}

			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			GUILayout.Label("Raw data", "BoldLabel");

			DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}
	}
	#endif
}
