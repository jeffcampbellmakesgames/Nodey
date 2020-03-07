using UnityEditor;
using UnityEngine;

namespace JCMG.Nodey.Editor
{
	public static class NodeEditorResources
	{
		public class Styles
		{
			public GUIStyle inputPort, nodeHeader, nodeBody, tooltip, nodeHighlight;

			public Styles()
			{
				var baseStyle = new GUIStyle("Label");
				baseStyle.fixedHeight = 18;

				inputPort = new GUIStyle(baseStyle);
				inputPort.alignment = TextAnchor.UpperLeft;
				inputPort.padding.left = 10;

				nodeHeader = new GUIStyle();
				nodeHeader.alignment = TextAnchor.MiddleCenter;
				nodeHeader.fontStyle = FontStyle.Bold;
				nodeHeader.normal.textColor = Color.white;

				nodeBody = new GUIStyle();
				nodeBody.normal.background = NodeEditorResources.nodeBody;
				nodeBody.border = new RectOffset(
					32,
					32,
					32,
					32);
				nodeBody.padding = new RectOffset(
					16,
					16,
					4,
					16);

				nodeHighlight = new GUIStyle();
				nodeHighlight.normal.background = NodeEditorResources.nodeHighlight;
				nodeHighlight.border = new RectOffset(
					32,
					32,
					32,
					32);

				tooltip = new GUIStyle("helpBox");
				tooltip.alignment = TextAnchor.MiddleCenter;
			}
		}

		// Textures
		public static Texture2D dot
		{
			get { return _dot != null ? _dot : _dot = Resources.Load<Texture2D>("nodey_dot"); }
		}

		public static Texture2D dotOuter
		{
			get { return _dotOuter != null ? _dotOuter : _dotOuter = Resources.Load<Texture2D>("nodey_dot_outer"); }
		}

		public static Texture2D nodeBody
		{
			get { return _nodeBody != null ? _nodeBody : _nodeBody = Resources.Load<Texture2D>("nodey_node"); }
		}

		public static Texture2D nodeHighlight
		{
			get
			{
				return _nodeHighlight != null
					? _nodeHighlight
					: _nodeHighlight = Resources.Load<Texture2D>("nodey_node_highlight");
			}
		}

		// Styles
		public static Styles styles
		{
			get { return _styles != null ? _styles : _styles = new Styles(); }
		}

		public static GUIStyle OutputPort
		{
			get
			{
				return new GUIStyle(EditorStyles.label)
				{
					alignment = TextAnchor.UpperRight
				};
			}
		}

		private static Texture2D _dot;
		private static Texture2D _dotOuter;
		private static Texture2D _nodeBody;
		private static Texture2D _nodeHighlight;
		public static Styles _styles;

		public static Texture2D GenerateGridTexture(Color line, Color bg)
		{
			var tex = new Texture2D(64, 64);
			var cols = new Color[64 * 64];
			for (var y = 0; y < 64; y++)
			{
				for (var x = 0; x < 64; x++)
				{
					var col = bg;
					if (y % 16 == 0 || x % 16 == 0)
					{
						col = Color.Lerp(line, bg, 0.65f);
					}

					if (y == 63 || x == 63)
					{
						col = Color.Lerp(line, bg, 0.35f);
					}

					cols[y * 64 + x] = col;
				}
			}

			tex.SetPixels(cols);
			tex.wrapMode = TextureWrapMode.Repeat;
			tex.filterMode = FilterMode.Bilinear;
			tex.name = "Grid";
			tex.Apply();
			return tex;
		}

		public static Texture2D GenerateCrossTexture(Color line)
		{
			var tex = new Texture2D(64, 64);
			var cols = new Color[64 * 64];
			for (var y = 0; y < 64; y++)
			{
				for (var x = 0; x < 64; x++)
				{
					var col = line;
					if (y != 31 && x != 31)
					{
						col.a = 0;
					}

					cols[y * 64 + x] = col;
				}
			}

			tex.SetPixels(cols);
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.filterMode = FilterMode.Bilinear;
			tex.name = "Grid";
			tex.Apply();
			return tex;
		}
	}
}
