using System;

namespace JCMG.Nodey
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeWidthAttribute : Attribute
	{
		public int width;

		/// <summary> Specify a width for this node type </summary>
		/// <param name = "width"> Width </param>
		public NodeWidthAttribute(int width)
		{
			this.width = width;
		}
	}
}
