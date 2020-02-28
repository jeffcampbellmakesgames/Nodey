using UnityEngine;
using xNode.Editor;

namespace Examples.StateMachine.Editor
{
	[CustomNodeEditor(typeof(StateNode))]
	public class StateNodeEditor : NodeEditor
	{
		public override void OnHeaderGUI()
		{
			GUI.color = Color.white;
			var node = target as StateNode;
			var graph = node.graph as StateGraph;
			if (graph.current == node)
			{
				GUI.color = Color.blue;
			}

			var title = target.name;
			GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
			GUI.color = Color.white;
		}

		public override void OnBodyGUI()
		{
			base.OnBodyGUI();
			var node = target as StateNode;
			var graph = node.graph as StateGraph;
			if (GUILayout.Button("MoveNext Node"))
			{
				node.MoveNext();
			}

			if (GUILayout.Button("Continue Graph"))
			{
				graph.Continue();
			}

			if (GUILayout.Button("Set as current"))
			{
				graph.current = node;
			}
		}
	}
}
