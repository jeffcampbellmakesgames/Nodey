using System;

namespace JCMG.Nodey.Editor
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomNodeEditorAttribute : Attribute,
	                                         NodeEditorBase<NodeEditor, CustomNodeEditorAttribute, Node>.INodeEditorAttrib
	{
		private readonly Type inspectedType;

		/// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
		/// <param name = "inspectedType"> Type that this editor can edit </param>
		public CustomNodeEditorAttribute(Type inspectedType)
		{
			this.inspectedType = inspectedType;
		}

		public Type GetInspectedType()
		{
			return inspectedType;
		}
	}
}
