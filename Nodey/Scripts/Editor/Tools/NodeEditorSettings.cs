using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JCMG.Nodey.Editor
{
	[Serializable]
	public class NodeEditorSettings : ISerializationCallbackReceiver
	{
		public Color32 gridLineColor
		{
			get { return _gridLineColor; }
			set
			{
				_gridLineColor = value;
				_gridTexture = null;
				_crossTexture = null;
			}
		}

		public Color32 gridBgColor
		{
			get { return _gridBgColor; }
			set
			{
				_gridBgColor = value;
				_gridTexture = null;
			}
		}

		[Obsolete("Use maxZoom instead")]
		public float zoomOutLimit
		{
			get { return maxZoom; }
			set { maxZoom = value; }
		}

		public Texture2D gridTexture
		{
			get
			{
				if (_gridTexture == null)
				{
					_gridTexture = NodeEditorResources.GenerateGridTexture(gridLineColor, gridBgColor);
				}

				return _gridTexture;
			}
		}

		public Texture2D crossTexture
		{
			get
			{
				if (_crossTexture == null)
				{
					_crossTexture = NodeEditorResources.GenerateCrossTexture(gridLineColor);
				}

				return _crossTexture;
			}
		}

		private Texture2D _crossTexture;

		[SerializeField]
		private Color32 _gridBgColor = new Color(0.18f, 0.18f, 0.18f);

		[SerializeField]
		private Color32 _gridLineColor = new Color(0.45f, 0.45f, 0.45f);

		private Texture2D _gridTexture;
		public bool autoSave = true;
		public bool dragToCreate = true;
		public bool gridSnap = true;

		public Color32 highlightColor = new Color32(
			255,
			255,
			255,
			255);

		[FormerlySerializedAs("zoomOutLimit")]
		public float maxZoom = 5f;

		public float minZoom = 1f;

		[FormerlySerializedAs("noodleType")]
		public NoodlePath noodlePath = NoodlePath.Curvy;

		public NoodleStroke noodleStroke = NoodleStroke.Full;
		public bool portTooltips = true;

		[NonSerialized]
		public Dictionary<string, Color> typeColors = new Dictionary<string, Color>();

		[SerializeField]
		private string typeColorsData = "";

		public bool zoomToMouse = true;

		public void OnAfterDeserialize()
		{
			// Deserialize typeColorsData
			typeColors = new Dictionary<string, Color>();
			var data = typeColorsData.Split(
				new[]
				{
					','
				},
				StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < data.Length; i += 2)
			{
				if (ColorUtility.TryParseHtmlString("#" + data[i + 1], out var col))
				{
					typeColors.Add(data[i], col);
				}
			}
		}

		public void OnBeforeSerialize()
		{
			// Serialize typeColors
			typeColorsData = "";
			foreach (var item in typeColors)
			{
				typeColorsData += item.Key + "," + ColorUtility.ToHtmlStringRGB(item.Value) + ",";
			}
		}
	}
}
