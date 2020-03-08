using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.Nodey.Editor
{
	[InitializeOnLoad]
	public partial class NodeEditorWindow : EditorWindow
	{
		[Serializable]
		private class NodePortReference
		{
			[SerializeField]
			private string _name;

			[SerializeField]
			private Node _node;

			public NodePortReference(NodePort nodePort)
			{
				_node = nodePort.node;
				_name = nodePort.fieldName;
			}

			public NodePort GetNodePort()
			{
				if (_node == null)
				{
					return null;
				}

				return _node.GetPort(_name);
			}
		}

		/// <summary> Stores node positions for all nodePorts. </summary>
		public Dictionary<NodePort, Rect> portConnectionPoints { get; } = new Dictionary<NodePort, Rect>();

		private Func<bool> isDocked
		{
			get
			{
				if (_isDocked == null)
				{
					_isDocked = this.GetIsDockedDelegate();
				}

				return _isDocked;
			}
		}

		public Dictionary<Node, Vector2> nodeSizes { get; } = new Dictionary<Node, Vector2>();

		public Vector2 panOffset
		{
			get { return _panOffset; }
			set
			{
				_panOffset = value;
				Repaint();
			}
		}

		public float zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = Mathf.Clamp(
					value,
					NodeEditorPreferences.GetSettings().minZoom,
					NodeEditorPreferences.GetSettings().maxZoom);
				Repaint();
			}
		}

		private Func<bool> _isDocked;
		private Vector2 _panOffset;

		[SerializeField]
		private Rect[] _rects = new Rect[0];

		[SerializeField]
		private NodePortReference[] _references = new NodePortReference[0];

		private float _zoom = 1;
		public NodeGraph graph;
		public static NodeEditorWindow current;

		private void OnDisable()
		{
			// Cache portConnectionPoints before serialization starts
			var count = portConnectionPoints.Count;
			_references = new NodePortReference[count];
			_rects = new Rect[count];
			var index = 0;
			foreach (var portConnectionPoint in portConnectionPoints)
			{
				_references[index] = new NodePortReference(portConnectionPoint.Key);
				_rects[index] = portConnectionPoint.Value;
				index++;
			}
		}

		private void OnEnable()
		{
			// Reload portConnectionPoints if there are any
			var length = _references.Length;
			if (length == _rects.Length)
			{
				for (var i = 0; i < length; i++)
				{
					var nodePort = _references[i].GetNodePort();
					if (nodePort != null)
					{
						portConnectionPoints.Add(nodePort, _rects[i]);
					}
				}
			}
		}

		private void OnFocus()
		{
			current = this;
			ValidateGraphEditor();
			if (graphEditor != null && NodeEditorPreferences.GetSettings().autoSave)
			{
				AssetDatabase.SaveAssets();
			}
		}

		[InitializeOnLoadMethod]
		private static void OnLoad()
		{
			Selection.selectionChanged -= OnSelectionChanged;
			Selection.selectionChanged += OnSelectionChanged;
		}

		/// <summary> Handle Selection Change events </summary>
		private static void OnSelectionChanged()
		{
			var nodeGraph = Selection.activeObject as NodeGraph;
			if (nodeGraph && !AssetDatabase.Contains(nodeGraph))
			{
				Open(nodeGraph);
			}
		}

		/// <summary> Make sure the graph editor is assigned and to the right object </summary>
		private void ValidateGraphEditor()
		{
			var newGraphEditor = NodeGraphEditor.GetEditor(graph, this);
			if (newGraphEditor != null && graphEditor != newGraphEditor)
			{
				graphEditor = newGraphEditor;
				newGraphEditor.OnOpen();
			}
		}

		/// <summary> Create editor window </summary>
		public static NodeEditorWindow Init()
		{
			var w = CreateInstance<NodeEditorWindow>();
			w.titleContent = new GUIContent("Nodey");
			w.wantsMouseMove = true;
			w.Show();
			return w;
		}

		public void Save()
		{
			if (AssetDatabase.Contains(graph))
			{
				EditorUtility.SetDirty(graph);
				if (NodeEditorPreferences.GetSettings().autoSave)
				{
					AssetDatabase.SaveAssets();
				}
			}
			else
			{
				SaveAs();
			}
		}

		public void SaveAs()
		{
			var path = EditorUtility.SaveFilePanelInProject(
				"Save NodeGraph",
				"NewNodeGraph",
				"asset",
				"");
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			var existingGraph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
			if (existingGraph != null)
			{
				AssetDatabase.DeleteAsset(path);
			}

			AssetDatabase.CreateAsset(graph, path);
			EditorUtility.SetDirty(graph);
			if (NodeEditorPreferences.GetSettings().autoSave)
			{
				AssetDatabase.SaveAssets();
			}
		}

		private void DraggableWindow(int windowID)
		{
			GUI.DragWindow();
		}

		public Vector2 WindowToGridPosition(Vector2 windowPosition)
		{
			return (windowPosition - position.size * 0.5f - panOffset / zoom) * zoom;
		}

		public Vector2 GridToWindowPosition(Vector2 gridPosition)
		{
			return position.size * 0.5f + panOffset / zoom + gridPosition / zoom;
		}

		public Rect GridToWindowRectNoClipped(Rect gridRect)
		{
			gridRect.position = GridToWindowPositionNoClipped(gridRect.position);
			return gridRect;
		}

		public Rect GridToWindowRect(Rect gridRect)
		{
			gridRect.position = GridToWindowPosition(gridRect.position);
			gridRect.size /= zoom;
			return gridRect;
		}

		public Vector2 GridToWindowPositionNoClipped(Vector2 gridPosition)
		{
			var center = position.size * 0.5f;
			// UI Sharpness complete fix - Round final offset not panOffset
			var xOffset = Mathf.Round(center.x * zoom + (panOffset.x + gridPosition.x));
			var yOffset = Mathf.Round(center.y * zoom + (panOffset.y + gridPosition.y));
			return new Vector2(xOffset, yOffset);
		}

		public void SelectNode(Node node, bool add)
		{
			if (add)
			{
				var selection = new List<Object>(Selection.objects);
				selection.Add(node);
				Selection.objects = selection.ToArray();
			}
			else
			{
				Selection.objects = new Object[]
				{
					node
				};
			}
		}

		public void DeselectNode(Node node)
		{
			var selection = new List<Object>(Selection.objects);
			selection.Remove(node);
			Selection.objects = selection.ToArray();
		}

		[OnOpenAsset(0)]
		public static bool OnOpen(int instanceID, int line)
		{
			var nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as NodeGraph;
			if (nodeGraph != null)
			{
				Open(nodeGraph);
				return true;
			}

			return false;
		}

		/// <summary> Open the provided graph in the NodeEditor </summary>
		public static NodeEditorWindow Open(NodeGraph graph)
		{
			if (!graph)
			{
				return null;
			}

			var w = GetWindow(
				typeof(NodeEditorWindow),
				false,
				"Nodey",
				true) as NodeEditorWindow;
			w.wantsMouseMove = true;
			w.graph = graph;
			return w;
		}

		/// <summary> Repaint all open NodeEditorWindows. </summary>
		public static void RepaintAll()
		{
			var windows = Resources.FindObjectsOfTypeAll<NodeEditorWindow>();
			for (var i = 0; i < windows.Length; i++)
			{
				windows[i].Repaint();
			}
		}
	}
}
