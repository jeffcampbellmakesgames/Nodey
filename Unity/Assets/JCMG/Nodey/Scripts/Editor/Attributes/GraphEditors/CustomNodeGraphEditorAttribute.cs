using System;

namespace JCMG.Nodey.Editor
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomNodeGraphEditorAttribute : Attribute,
	                                              NodeEditorBase<NodeGraphEditor, CustomNodeGraphEditorAttribute, NodeGraph>.INodeEditorAttrib
	{
		public string editorPrefsKey;
		private readonly Type inspectedType;

		/// <summary> Tells a NodeGraphEditor which Graph type it is an editor for </summary>
		/// <param name = "inspectedType"> Type that this editor can edit </param>
		/// <param name = "editorPrefsKey"> Define unique key for unique layout settings instance </param>
		public CustomNodeGraphEditorAttribute(Type inspectedType, string editorPrefsKey = "Nodey.Settings")
		{
			this.inspectedType = inspectedType;
			this.editorPrefsKey = editorPrefsKey;
		}

		public Type GetInspectedType()
		{
			return inspectedType;
		}
	}
}
