using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.Nodey.Editor
{
	/// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
	[CustomNodeGraphEditor(typeof(NodeGraph))]
	public class NodeGraphEditor : NodeEditorBase<NodeGraphEditor, CustomNodeGraphEditorAttribute, NodeGraph>
	{
		[Obsolete("Use window.position instead")]
		public Rect position
		{
			get { return window.position; }
			set { window.position = value; }
		}

		/// <summary> Are we currently renaming a node? </summary>
		protected bool isRenaming;

		public virtual void OnGUI()
		{
		}

		/// <summary> Called when opened by NodeEditorWindow </summary>
		public virtual void OnOpen()
		{
		}

		public virtual Texture2D GetGridTexture()
		{
			return NodeEditorPreferences.GetSettings().gridTexture;
		}

		public virtual Texture2D GetSecondaryGridTexture()
		{
			return NodeEditorPreferences.GetSettings().crossTexture;
		}

		/// <summary>
		///     Return default settings for this graph type. This is the settings the user will load if no previous settings
		///     have been saved.
		/// </summary>
		public virtual NodeEditorSettings GetDefaultPreferences()
		{
			return new NodeEditorSettings();
		}

		/// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
		public virtual string GetNodeMenuName(Type type)
		{
			//Check if type has the CreateNodeMenuAttribute
			if (NodeEditorUtilities.GetAttrib(type, out CreateNodeMenuAttribute attrib)) // Return custom path
			{
				return attrib.menuName;
			}

			return NodeEditorUtilities.NodeDefaultPath(type);
		}

		/// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
		public virtual void AddContextMenuItems(GenericMenu menu)
		{
			var pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
			for (var i = 0; i < NodeEditorReflection.nodeTypes.Length; i++)
			{
				var type = NodeEditorReflection.nodeTypes[i];

				//Get node context menu path
				var path = GetNodeMenuName(type);
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}

				menu.AddItem(
					new GUIContent(path),
					false,
					() =>
					{
						var node = CreateNode(type, pos);
						NodeEditorWindow.current.AutoConnect(node);
					});
			}

			menu.AddSeparator("");
			if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0)
			{
				menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Paste"));
			}

			menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
			menu.AddCustomContextMenuItems(target);
		}

		/// <summary> Returned gradient is used to color noodles </summary>
		/// <param name = "output"> The output this noodle comes from. Never null. </param>
		/// <param name = "input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
		public virtual Gradient GetNoodleGradient(NodePort output, NodePort input)
		{
			var grad = new Gradient();

			// If dragging the noodle, draw solid, slightly transparent
			if (input == null)
			{
				var a = GetTypeColor(output.ValueType);
				grad.SetKeys(
					new[]
					{
						new GradientColorKey(a, 0f)
					},
					new[]
					{
						new GradientAlphaKey(0.6f, 0f)
					});
			}
			// If normal, draw gradient fading from one input color to the other
			else
			{
				var a = GetTypeColor(output.ValueType);
				var b = GetTypeColor(input.ValueType);
				// If any port is hovered, tint white
				if (window.hoveredPort == output || window.hoveredPort == input)
				{
					a = Color.Lerp(a, Color.white, 0.8f);
					b = Color.Lerp(b, Color.white, 0.8f);
				}

				grad.SetKeys(
					new[]
					{
						new GradientColorKey(a, 0f),
						new GradientColorKey(b, 1f)
					},
					new[]
					{
						new GradientAlphaKey(1f, 0f),
						new GradientAlphaKey(1f, 1f)
					});
			}

			return grad;
		}

		/// <summary> Returned float is used for noodle thickness </summary>
		/// <param name = "output"> The output this noodle comes from. Never null. </param>
		/// <param name = "input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
		public virtual float GetNoodleThickness(NodePort output, NodePort input)
		{
			return 5f;
		}

		public virtual NoodlePath GetNoodlePath(NodePort output, NodePort input)
		{
			return NodeEditorPreferences.GetSettings().noodlePath;
		}

		public virtual NoodleStroke GetNoodleStroke(NodePort output, NodePort input)
		{
			return NodeEditorPreferences.GetSettings().noodleStroke;
		}

		/// <summary> Returned color is used to color ports </summary>
		public virtual Color GetPortColor(NodePort port)
		{
			return GetTypeColor(port.ValueType);
		}

		/// <summary> Returns generated color for a type. This color is editable in preferences </summary>
		public virtual Color GetTypeColor(Type type)
		{
			return NodeEditorPreferences.GetTypeColor(type);
		}

		/// <summary> Override to display custom tooltips </summary>
		public virtual string GetPortTooltip(NodePort port)
		{
			var portType = port.ValueType;
			var tooltip = "";
			tooltip = portType.PrettyName();
			if (port.IsOutput)
			{
				var obj = port.node.GetValue(port);
				tooltip += " = " + (obj != null ? obj.ToString() : "null");
			}

			return tooltip;
		}

		/// <summary> Deal with objects dropped into the graph through DragAndDrop </summary>
		public virtual void OnDropObjects(Object[] objects)
		{
			Debug.Log("No OnDropObjects override defined for " + GetType());
		}

		/// <summary> Create a node and save it in the graph asset </summary>
		public virtual Node CreateNode(Type type, Vector2 position)
		{
			Undo.RecordObject(target, "Create Node");
			var node = target.AddNode(type);
			Undo.RegisterCreatedObjectUndo(node, "Create Node");
			node.position = position;
			if (node.name == null || node.name.Trim() == "")
			{
				node.name = NodeEditorUtilities.NodeDefaultName(type);
			}

			AssetDatabase.AddObjectToAsset(node, target);
			if (NodeEditorPreferences.GetSettings().autoSave)
			{
				AssetDatabase.SaveAssets();
			}

			NodeEditorWindow.RepaintAll();
			return node;
		}

		/// <summary> Creates a copy of the original node in the graph </summary>
		public Node CopyNode(Node original)
		{
			Undo.RecordObject(target, "Duplicate Node");
			var node = target.CopyNode(original);
			Undo.RegisterCreatedObjectUndo(node, "Duplicate Node");
			node.name = original.name;
			AssetDatabase.AddObjectToAsset(node, target);
			if (NodeEditorPreferences.GetSettings().autoSave)
			{
				AssetDatabase.SaveAssets();
			}

			return node;
		}

		/// <summary> Safely remove a node and all its connections. </summary>
		public virtual void RemoveNode(Node node)
		{
			Undo.RecordObject(node, "Delete Node");
			Undo.RecordObject(target, "Delete Node");
			foreach (var port in node.Ports)
			{
				foreach (var conn in port.GetConnections())
				{
					Undo.RecordObject(conn.node, "Delete Node");
				}
			}

			target.RemoveNode(node);
			Undo.DestroyObjectImmediate(node);
			if (NodeEditorPreferences.GetSettings().autoSave)
			{
				AssetDatabase.SaveAssets();
			}
		}
	}
}
