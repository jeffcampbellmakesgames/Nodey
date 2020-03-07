using System;
using JCMG.Nodey;
using UnityEngine;

namespace Examples.StateMachine
{
	public class StateNode : Node
	{
		[Serializable]
		public class Empty
		{
		}

		[Input]
		public Empty enter;

		[Output]
		public Empty exit;

		public void MoveNext()
		{
			var fmGraph = graph as StateGraph;

			if (fmGraph.current != this)
			{
				Debug.LogWarning("Node isn't active");
				return;
			}

			var exitPort = GetOutputPort("exit");

			if (!exitPort.IsConnected)
			{
				Debug.LogWarning("Node isn't connected");
				return;
			}

			var node = exitPort.Connection.node as StateNode;
			node.OnEnter();
		}

		public void OnEnter()
		{
			var fmGraph = graph as StateGraph;
			fmGraph.current = this;
		}
	}
}
