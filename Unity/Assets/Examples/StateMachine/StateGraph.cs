using JCMG.Nodey;
using UnityEngine;

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
