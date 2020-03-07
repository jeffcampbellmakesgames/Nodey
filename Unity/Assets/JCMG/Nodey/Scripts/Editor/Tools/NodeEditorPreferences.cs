using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JCMG.Nodey.Editor
{
	public static class NodeEditorPreferences
	{
		/// <summary> The last editor we checked. This should be the one we modify </summary>
		private static NodeGraphEditor lastEditor;

		/// <summary> The last key we checked. This should be the one we modify </summary>
		private static string lastKey = "Nodey.Settings";

		private static Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
		private static readonly Dictionary<string, NodeEditorSettings> settings = new Dictionary<string, NodeEditorSettings>();

		/// <summary> Get settings of current active editor </summary>
		public static NodeEditorSettings GetSettings()
		{
			if (NodeEditorWindow.current == null)
			{
				return new NodeEditorSettings();
			}

			if (lastEditor != NodeEditorWindow.current.graphEditor)
			{
				var attribs = NodeEditorWindow.current.graphEditor.GetType()
					.GetCustomAttributes(typeof(CustomNodeGraphEditorAttribute), true);
				if (attribs.Length == 1)
				{
					var attrib = attribs[0] as CustomNodeGraphEditorAttribute;
					lastEditor = NodeEditorWindow.current.graphEditor;
					lastKey = attrib.editorPrefsKey;
				}
				else
				{
					return null;
				}
			}

			if (!settings.ContainsKey(lastKey))
			{
				VerifyLoaded();
			}

			return settings[lastKey];
		}

		#if UNITY_2019_1_OR_NEWER
		[SettingsProvider]
		public static SettingsProvider CreateNodeySettingsProvider()
		{
			var provider = new SettingsProvider("Preferences/JCMG Nodey Editor", SettingsScope.User)
			{
				guiHandler = searchContext =>
				{
					PreferencesGUI();
				},
				keywords = new HashSet<string>(
					new[]
					{
						"JCMG",
						"Nodey",
						"node",
						"editor",
						"graph",
						"connections",
						"noodles",
						"ports"
					})
			};
			return provider;
		}
		#endif

		#if !UNITY_2019_1_OR_NEWER
        [PreferenceItem("JCMG Nodey Editor")]
		#endif
		private static void PreferencesGUI()
		{
			VerifyLoaded();
			var settings = NodeEditorPreferences.settings[lastKey];

			EditorGUILayout.Space();

			NodeSettingsGUI(lastKey, settings);
			GridSettingsGUI(lastKey, settings);
			SystemSettingsGUI(lastKey, settings);
			TypeColorsGUI(lastKey, settings);
			if (GUILayout.Button(new GUIContent("Set Default", "Reset all values to default"), GUILayout.Width(120)))
			{
				ResetPrefs();
			}
		}

		private static void GridSettingsGUI(string key, NodeEditorSettings nodeEditorSettings)
		{
			//Label
			EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
			nodeEditorSettings.gridSnap = EditorGUILayout.Toggle(
				new GUIContent("Snap", "Hold CTRL in editor to invert"),
				nodeEditorSettings.gridSnap);
			nodeEditorSettings.zoomToMouse = EditorGUILayout.Toggle(
				new GUIContent("Zoom to Mouse", "Zooms towards mouse position"),
				nodeEditorSettings.zoomToMouse);
			EditorGUILayout.LabelField("Zoom");
			EditorGUI.indentLevel++;
			nodeEditorSettings.maxZoom = EditorGUILayout.FloatField(new GUIContent("Max", "Upper limit to zoom"), nodeEditorSettings.maxZoom);
			nodeEditorSettings.minZoom = EditorGUILayout.FloatField(new GUIContent("Min", "Lower limit to zoom"), nodeEditorSettings.minZoom);
			EditorGUI.indentLevel--;
			nodeEditorSettings.gridLineColor = EditorGUILayout.ColorField("Color", nodeEditorSettings.gridLineColor);
			nodeEditorSettings.gridBgColor = EditorGUILayout.ColorField(" ", nodeEditorSettings.gridBgColor);
			if (GUI.changed)
			{
				SavePrefs(key, nodeEditorSettings);

				NodeEditorWindow.RepaintAll();
			}

			EditorGUILayout.Space();
		}

		private static void SystemSettingsGUI(string key, NodeEditorSettings nodeEditorSettings)
		{
			//Label
			EditorGUILayout.LabelField("System", EditorStyles.boldLabel);
			nodeEditorSettings.autoSave = EditorGUILayout.Toggle(
				new GUIContent("Autosave", "Disable for better editor performance"),
				nodeEditorSettings.autoSave);
			if (GUI.changed)
			{
				SavePrefs(key, nodeEditorSettings);
			}

			EditorGUILayout.Space();
		}

		private static void NodeSettingsGUI(string key, NodeEditorSettings nodeEditorSettings)
		{
			//Label
			EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
			nodeEditorSettings.highlightColor = EditorGUILayout.ColorField("Selection", nodeEditorSettings.highlightColor);
			nodeEditorSettings.noodlePath = (NoodlePath)EditorGUILayout.EnumPopup("Noodle path", nodeEditorSettings.noodlePath);
			nodeEditorSettings.noodleStroke = (NoodleStroke)EditorGUILayout.EnumPopup("Noodle stroke", nodeEditorSettings.noodleStroke);
			nodeEditorSettings.portTooltips = EditorGUILayout.Toggle("Port Tooltips", nodeEditorSettings.portTooltips);
			nodeEditorSettings.dragToCreate = EditorGUILayout.Toggle(
				new GUIContent("Drag to Create", "Drag a port connection anywhere on the grid to create and connect a node"),
				nodeEditorSettings.dragToCreate);
			if (GUI.changed)
			{
				SavePrefs(key, nodeEditorSettings);
				NodeEditorWindow.RepaintAll();
			}

			EditorGUILayout.Space();
		}

		private static void TypeColorsGUI(string key, NodeEditorSettings nodeEditorSettings)
		{
			//Label
			EditorGUILayout.LabelField("Types", EditorStyles.boldLabel);

			//Clone keys so we can enumerate the dictionary and make changes.
			var typeColorKeys = new List<Type>(typeColors.Keys);

			//Display type colors. Save them if they are edited by the user
			foreach (var type in typeColorKeys)
			{
				var typeColorKey = type.PrettyName();
				var col = typeColors[type];
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				col = EditorGUILayout.ColorField(typeColorKey, col);
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					typeColors[type] = col;
					if (nodeEditorSettings.typeColors.ContainsKey(typeColorKey))
					{
						nodeEditorSettings.typeColors[typeColorKey] = col;
					}
					else
					{
						nodeEditorSettings.typeColors.Add(typeColorKey, col);
					}

					SavePrefs(key, nodeEditorSettings);
					NodeEditorWindow.RepaintAll();
				}
			}
		}

		/// <summary> Load prefs if they exist. Create if they don't </summary>
		private static NodeEditorSettings LoadPrefs()
		{
			// Create settings if it doesn't exist
			if (!EditorPrefs.HasKey(lastKey))
			{
				if (lastEditor != null)
				{
					EditorPrefs.SetString(lastKey, JsonUtility.ToJson(lastEditor.GetDefaultPreferences()));
				}
				else
				{
					EditorPrefs.SetString(lastKey, JsonUtility.ToJson(new NodeEditorSettings()));
				}
			}

			return JsonUtility.FromJson<NodeEditorSettings>(EditorPrefs.GetString(lastKey));
		}

		/// <summary> Delete all prefs </summary>
		public static void ResetPrefs()
		{
			if (EditorPrefs.HasKey(lastKey))
			{
				EditorPrefs.DeleteKey(lastKey);
			}

			if (settings.ContainsKey(lastKey))
			{
				settings.Remove(lastKey);
			}

			typeColors = new Dictionary<Type, Color>();
			VerifyLoaded();
			NodeEditorWindow.RepaintAll();
		}

		/// <summary> Save preferences in EditorPrefs </summary>
		private static void SavePrefs(string key, NodeEditorSettings nodeEditorSettings)
		{
			EditorPrefs.SetString(key, JsonUtility.ToJson(nodeEditorSettings));
		}

		/// <summary> Check if we have loaded settings for given key. If not, load them </summary>
		private static void VerifyLoaded()
		{
			if (!settings.ContainsKey(lastKey))
			{
				settings.Add(lastKey, LoadPrefs());
			}
		}

		/// <summary> Return color based on type </summary>
		public static Color GetTypeColor(Type type)
		{
			VerifyLoaded();
			if (type == null)
			{
				return Color.gray;
			}

			if (!typeColors.TryGetValue(type, out var col))
			{
				var typeName = type.PrettyName();
				if (settings[lastKey].typeColors.ContainsKey(typeName))
				{
					typeColors.Add(type, settings[lastKey].typeColors[typeName]);
				}
				else
				{
					#if UNITY_5_4_OR_NEWER
					var oldState = Random.state;
					Random.InitState(typeName.GetHashCode());
					#else
                    int oldSeed = UnityEngine.Random.seed;
                    UnityEngine.Random.seed = typeName.GetHashCode();
					#endif
					col = new Color(Random.value, Random.value, Random.value);
					typeColors.Add(type, col);
					#if UNITY_5_4_OR_NEWER
					Random.state = oldState;
					#else
                    UnityEngine.Random.seed = oldSeed;
					#endif
				}
			}

			return col;
		}
	}
}
