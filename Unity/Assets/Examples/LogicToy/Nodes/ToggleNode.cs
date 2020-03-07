using System.Linq;
using JCMG.Nodey;
using UnityEngine;

namespace Examples.LogicToy
{
	[NodeWidth(140)]
	[NodeTint(70, 70, 100)]
	public class ToggleNode : LogicNode
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
			var newInput = GetPort("input").GetInputValues<bool>().Any(x => x);

			if (!input && newInput)
			{
				input = newInput;
				output = !output;
				SendSignal(GetPort("output"));
			}
			else if (input && !newInput)
			{
				input = newInput;
			}
		}

		public override object GetValue(NodePort port)
		{
			return output;
		}
	}
}
