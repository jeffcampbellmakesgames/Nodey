using System;
using JCMG.Nodey;

namespace Examples.MathGraph
{
	public class DisplayValue : Node
	{
		/// <summary> This class is defined for the sole purpose of being serializable </summary>
		[Serializable]
		public class Anything
		{
		}

		/// <summary>
		///     Create an input port that only allows a single connection. The backing value is not important, as we are only
		///     interested in the input value. We are also acceptable of all input types, so any type will do, as long as it is
		///     serializable.
		/// </summary>
		[Input(ShowBackingValue.Never, ConnectionType.Override)]
		public Anything input;

		/// <summary> Get the value currently plugged in to this node </summary>
		public object GetValue()
		{
			return GetInputValue<object>("input");
		}
	}
}
