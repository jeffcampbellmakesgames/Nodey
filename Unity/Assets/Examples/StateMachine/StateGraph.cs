using UnityEngine;
using XNode;

namespace Examples.StateMachine
{
	[CreateAssetMenu(fileName = "New State Graph", menuName = "xNode Examples/State Graph")]
	public class StateGraph : NodeGraph
	{
		// The current "active" node
		public StateNode current;

		public void Continue()
		{
			current.MoveNext();
		}
	}
}
