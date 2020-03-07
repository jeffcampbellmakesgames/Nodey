using JCMG.Nodey;
using UnityEngine;

namespace Examples.LogicToy
{
	[NodeWidth(140)]
	[NodeTint(70, 100, 70)]
	public class PulseNode : LogicNode,
	                         ITimerTick
	{
		public override bool led
		{
			get { return output; }
		}

		[Space(-18)]
		public float interval = 1f;

		[Output]
		[HideInInspector]
		public bool output;

		private float timer;

		/// <summary> This node can not receive signals, so this is not used </summary>
		protected override void OnInputChanged()
		{
		}

		public override object GetValue(NodePort port)
		{
			return output;
		}

		public void Tick(float deltaTime)
		{
			timer += deltaTime;
			if (!output && timer > interval)
			{
				timer -= interval;
				output = true;
				SendSignal(GetPort("output"));
			}
			else if (output)
			{
				output = false;
				SendSignal(GetPort("output"));
			}
		}
	}
}
