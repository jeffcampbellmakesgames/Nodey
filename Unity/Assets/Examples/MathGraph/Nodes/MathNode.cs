using System;
using JCMG.Nodey;

namespace Examples.MathGraph
{
	[Serializable]
	public class MathNode : Node
	{
		public enum MathType
		{
			Add,
			Subtract,
			Multiply,
			Divide
		}

		// Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node
		[Input]
		public float a;

		[Input]
		public float b;

		// Will be displayed as an editable field - just like the normal inspector
		public MathType mathType = MathType.Add;

		// The value of an output node field is not used for anything, but could be used for caching output results
		[Output]
		public float result;

		// GetValue should be overridden to return a value for any specified output port
		public override object GetValue(NodePort port)
		{
			// Get new a and b values from input connections. Fallback to field values if input is not connected
			var a = GetInputValue("a", this.a);
			var b = GetInputValue("b", this.b);

			// After you've gotten your input values, you can perform your calculations and return a value
			result = 0f;
			if (port.fieldName == "result")
			{
				switch (mathType)
				{
					case MathType.Add:
					default:
						result = a + b;
						break;
					case MathType.Subtract:
						result = a - b;
						break;
					case MathType.Multiply:
						result = a * b;
						break;
					case MathType.Divide:
						result = a / b;
						break;
				}
			}

			return result;
		}
	}
}
