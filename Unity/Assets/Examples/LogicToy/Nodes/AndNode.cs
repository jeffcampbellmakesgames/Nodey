using System.Linq;
using UnityEngine;
using XNode;

namespace Examples.LogicToy
{
	[NodeWidth(140)]
	[NodeTint(100, 70, 70)]
	public class AndNode : LogicNode
	{
		public override bool led
		{
			get { return output; }
		}

		[Input]
		[HideInInspector]
		public bool input;

		[Output]
		[HideInInspector]
		public bool output;

		protected override void OnInputChanged()
		{
			var newInput = GetPort("input").GetInputValues<bool>().All(x => x);

			if (input != newInput)
			{
				input = newInput;
				output = newInput;
				SendSignal(GetPort("output"));
			}
		}

		public override object GetValue(NodePort port)
		{
			return output;
		}
	}
}
