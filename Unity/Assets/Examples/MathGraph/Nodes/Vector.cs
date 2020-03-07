using JCMG.Nodey;
using UnityEngine;

namespace Examples.MathGraph
{
	public class Vector : Node
	{
		[Output]
		public Vector3 vector;

		[Input]
		public float x, y, z;

		public override object GetValue(NodePort port)
		{
			vector.x = GetInputValue("x", x);
			vector.y = GetInputValue("y", y);
			vector.z = GetInputValue("z", z);
			return vector;
		}
	}
}
