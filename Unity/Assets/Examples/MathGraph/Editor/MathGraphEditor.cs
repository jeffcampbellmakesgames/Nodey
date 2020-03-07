using System;
using JCMG.Nodey.Editor;

namespace Examples.MathGraph.Editor
{
	[CustomNodeGraphEditor(typeof(MathGraph))]
	public class MathGraphEditor : NodeGraphEditor
	{
		/// <summary>
		///     Overriding GetNodeMenuName lets you control if and how nodes are categorized. In this example we are sorting
		///     out all node types that are not in the XNode.Examples namespace.
		/// </summary>
		public override string GetNodeMenuName(Type type)
		{
			if (type.Namespace == "XNode.Examples.MathNodes")
			{
				return base.GetNodeMenuName(type).Replace("X Node/Examples/Math Nodes/", "");
			}

			return null;
		}
	}
}
