using System.Linq;
using JCMG.Nodey;
using UnityEngine;

namespace Examples.LogicToy
{
	[NodeWidth(140)]
	[NodeTint(100, 100, 50)]
	public class NotNode : LogicNode
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
		public bool output = true;

		protected override void OnInputChanged()
		{
			var newInput = GetPort("input").GetInputValues<bool>().Any(x => x);

			if (input != newInput)
			{
				input = newInput;
				output = !newInput;
				SendSignal(GetPort("output"));
			}
		}

		public override object GetValue(NodePort port)
		{
			return output;
		}
	}
}
