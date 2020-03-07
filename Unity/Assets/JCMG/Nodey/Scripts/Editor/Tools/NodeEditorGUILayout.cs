using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.Nodey.Editor
{
	/// <summary> xNode-specific version of <see cref = "EditorGUILayout"/> </summary>
	public static class NodeEditorGUILayout
	{
		private static readonly Dictionary<Object, Dictionary<string, ReorderableList>> reorderableListCache =
			new Dictionary<Object, Dictionary<string, ReorderableList>>();

		private static int reorderableListIndex = -1;

		/// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
		public static void PropertyField(SerializedProperty property, bool includeChildren = true,
		                                 params GUILayoutOption[] options)
		{
			PropertyField(
				property,
				(GUIContent)null,
				includeChildren,
				options);
		}

		/// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
		public static void PropertyField(SerializedProperty property, GUIContent label, bool includeChildren = true,
		                                 params GUILayoutOption[] options)
		{
			if (property == null)
			{
				throw new NullReferenceException();
			}

			var node = property.serializedObject.targetObject as Node;
			var port = node.GetPort(property.name);
			PropertyField(
				property,
				label,
				port,
				includeChildren);
		}

		/// <summary> Make a field for a serialized property. Manual node port override. </summary>
		public static void PropertyField(SerializedProperty property, NodePort port, bool includeChildren = true,
		                                 params GUILayoutOption[] options)
		{
			PropertyField(
				property,
				null,
				port,
				includeChildren,
				options);
		}

		/// <summary> Make a field for a serialized property. Manual node port override. </summary>
		public static void PropertyField(SerializedProperty property, GUIContent label, NodePort port,
		                                 bool includeChildren = true, params GUILayoutOption[] options)
		{
			if (property == null)
			{
				throw new NullReferenceException();
			}

			// If property is not a port, display a regular property field
			if (port == null)
			{
				EditorGUILayout.PropertyField(
					property,
					label,
					includeChildren,
					GUILayout.MinWidth(30));
			}
			else
			{
				var rect = new Rect();

				var propertyAttributes = NodeEditorUtilities.GetCachedPropertyAttribs(port.node.GetType(), property.name);

				// If property is an input, display a regular property field and put a port handle on the left side
				if (port.direction == PortDirectionType.Input)
				{
					// Get data from [Input] attribute
					var showBacking = ShowBackingValue.Unconnected;
					var dynamicPortList = false;
					if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out InputAttribute inputAttribute))
					{
						dynamicPortList = inputAttribute.dynamicPortList;
						showBacking = inputAttribute.backingValue;
					}

					var usePropertyAttributes = dynamicPortList ||
					                            showBacking == ShowBackingValue.Never ||
					                            showBacking == ShowBackingValue.Unconnected && port.IsConnected;

					float spacePadding = 0;
					foreach (var attr in propertyAttributes)
					{
						if (attr is SpaceAttribute)
						{
							if (usePropertyAttributes)
							{
								GUILayout.Space((attr as SpaceAttribute).height);
							}
							else
							{
								spacePadding += (attr as SpaceAttribute).height;
							}
						}
						else if (attr is HeaderAttribute)
						{
							if (usePropertyAttributes)
							{
								//GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
								var position = GUILayoutUtility.GetRect(
									0,
									EditorGUIUtility.singleLineHeight * 1.5f -
									EditorGUIUtility
										.standardVerticalSpacing); //Layout adds standardVerticalSpacing after rect so we subtract it.
								position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
								position = EditorGUI.IndentedRect(position);
								GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
							}
							else
							{
								spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
							}
						}
					}

					if (dynamicPortList)
					{
						var type = GetType(property);
						var connectionType =
							inputAttribute != null ? inputAttribute.connectionType : ConnectionType.Multiple;
						DynamicPortList(
							property.name,
							type,
							property.serializedObject,
							port.direction,
							connectionType);
						return;
					}

					switch (showBacking)
					{
						case ShowBackingValue.Unconnected:
							// Display a label if port is connected
							if (port.IsConnected)
							{
								EditorGUILayout.LabelField(label != null ? label : new GUIContent(property.displayName));
							}
							// Display an editable property field if port is not connected
							else
							{
								EditorGUILayout.PropertyField(
									property,
									label,
									includeChildren,
									GUILayout.MinWidth(30));
							}

							break;
						case ShowBackingValue.Never:
							// Display a label
							EditorGUILayout.LabelField(label != null ? label : new GUIContent(property.displayName));
							break;
						case ShowBackingValue.Always:
							// Display an editable property field
							EditorGUILayout.PropertyField(
								property,
								label,
								includeChildren,
								GUILayout.MinWidth(30));
							break;
					}

					rect = GUILayoutUtility.GetLastRect();
					rect.position = rect.position - new Vector2(16, -spacePadding);
					// If property is an output, display a text label and put a port handle on the right side
				}
				else if (port.direction == PortDirectionType.Output)
				{
					// Get data from [Output] attribute
					var showBacking = ShowBackingValue.Unconnected;
					var dynamicPortList = false;
					if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out OutputAttribute outputAttribute))
					{
						dynamicPortList = outputAttribute.dynamicPortList;
						showBacking = outputAttribute.backingValue;
					}

					var usePropertyAttributes = dynamicPortList ||
					                            showBacking == ShowBackingValue.Never ||
					                            showBacking == ShowBackingValue.Unconnected && port.IsConnected;

					float spacePadding = 0;
					foreach (var attr in propertyAttributes)
					{
						if (attr is SpaceAttribute)
						{
							if (usePropertyAttributes)
							{
								GUILayout.Space((attr as SpaceAttribute).height);
							}
							else
							{
								spacePadding += (attr as SpaceAttribute).height;
							}
						}
						else if (attr is HeaderAttribute)
						{
							if (usePropertyAttributes)
							{
								//GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
								var position = GUILayoutUtility.GetRect(
									0,
									EditorGUIUtility.singleLineHeight * 1.5f -
									EditorGUIUtility
										.standardVerticalSpacing); //Layout adds standardVerticalSpacing after rect so we subtract it.
								position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
								position = EditorGUI.IndentedRect(position);
								GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
							}
							else
							{
								spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
							}
						}
					}

					if (dynamicPortList)
					{
						var type = GetType(property);
						var connectionType = outputAttribute != null
							? outputAttribute.connectionType
							: ConnectionType.Multiple;
						DynamicPortList(
							property.name,
							type,
							property.serializedObject,
							port.direction,
							connectionType);
						return;
					}

					switch (showBacking)
					{
						case ShowBackingValue.Unconnected:
							// Display a label if port is connected
							if (port.IsConnected)
							{
								EditorGUILayout.LabelField(
									label != null ? label : new GUIContent(property.displayName),
									NodeEditorResources.OutputPort,
									GUILayout.MinWidth(30));
							}
							// Display an editable property field if port is not connected
							else
							{
								EditorGUILayout.PropertyField(
									property,
									label,
									includeChildren,
									GUILayout.MinWidth(30));
							}

							break;
						case ShowBackingValue.Never:
							// Display a label
							EditorGUILayout.LabelField(
								label != null ? label : new GUIContent(property.displayName),
								NodeEditorResources.OutputPort,
								GUILayout.MinWidth(30));
							break;
						case ShowBackingValue.Always:
							// Display an editable property field
							EditorGUILayout.PropertyField(
								property,
								label,
								includeChildren,
								GUILayout.MinWidth(30));
							break;
					}

					rect = GUILayoutUtility.GetLastRect();
					rect.position = rect.position + new Vector2(rect.width, spacePadding);
				}

				rect.size = new Vector2(16, 16);

				var editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
				var backgroundColor = editor.GetTint();
				var col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
				DrawPortHandle(rect, backgroundColor, col);

				// Register the handle position
				var portPos = rect.center;
				NodeEditor.portPositions[port] = portPos;
			}
		}

		private static Type GetType(SerializedProperty property)
		{
			var parentType = property.serializedObject.targetObject.GetType();
			var fi = parentType.GetFieldInfo(property.name);
			return fi.FieldType;
		}

		/// <summary> Make a simple port field. </summary>
		public static void PortField(NodePort port, params GUILayoutOption[] options)
		{
			PortField(null, port, options);
		}

		/// <summary> Make a simple port field. </summary>
		public static void PortField(GUIContent label, NodePort port, params GUILayoutOption[] options)
		{
			if (port == null)
			{
				return;
			}

			if (options == null)
			{
				options = new[]
				{
					GUILayout.MinWidth(30)
				};
			}

			Vector2 position = Vector3.zero;
			var content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

			// If property is an input, display a regular property field and put a port handle on the left side
			if (port.direction == PortDirectionType.Input)
			{
				// Display a label
				EditorGUILayout.LabelField(content, options);

				var rect = GUILayoutUtility.GetLastRect();
				position = rect.position - new Vector2(16, 0);
			}
			// If property is an output, display a text label and put a port handle on the right side
			else if (port.direction == PortDirectionType.Output)
			{
				// Display a label
				EditorGUILayout.LabelField(content, NodeEditorResources.OutputPort, options);

				var rect = GUILayoutUtility.GetLastRect();
				position = rect.position + new Vector2(rect.width, 0);
			}

			PortField(position, port);
		}

		/// <summary> Make a simple port field. </summary>
		public static void PortField(Vector2 position, NodePort port)
		{
			if (port == null)
			{
				return;
			}

			var rect = new Rect(position, new Vector2(16, 16));

			var editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
			var backgroundColor = editor.GetTint();
			var col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
			DrawPortHandle(rect, backgroundColor, col);

			// Register the handle position
			var portPos = rect.center;
			NodeEditor.portPositions[port] = portPos;
		}

		/// <summary> Add a port field to previous layout element. </summary>
		public static void AddPortField(NodePort port)
		{
			if (port == null)
			{
				return;
			}

			var rect = new Rect();

			// If property is an input, display a regular property field and put a port handle on the left side
			if (port.direction == PortDirectionType.Input)
			{
				rect = GUILayoutUtility.GetLastRect();
				rect.position = rect.position - new Vector2(16, 0);
				// If property is an output, display a text label and put a port handle on the right side
			}
			else if (port.direction == PortDirectionType.Output)
			{
				rect = GUILayoutUtility.GetLastRect();
				rect.position = rect.position + new Vector2(rect.width, 0);
			}

			rect.size = new Vector2(16, 16);

			var editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
			var backgroundColor = editor.GetTint();
			var col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
			DrawPortHandle(rect, backgroundColor, col);

			// Register the handle position
			var portPos = rect.center;
			NodeEditor.portPositions[port] = portPos;
		}

		/// <summary> Draws an input and an output port on the same line </summary>
		public static void PortPair(NodePort input, NodePort output)
		{
			GUILayout.BeginHorizontal();
			PortField(input, GUILayout.MinWidth(0));
			PortField(output, GUILayout.MinWidth(0));
			GUILayout.EndHorizontal();
		}

		public static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
		{
			var col = GUI.color;
			GUI.color = backgroundColor;
			GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
			GUI.color = typeColor;
			GUI.DrawTexture(rect, NodeEditorResources.dot);
			GUI.color = col;
		}

		/// <summary> Is this port part of a DynamicPortList? </summary>
		public static bool IsDynamicPortListPort(NodePort port)
		{
			var parts = port.fieldName.Split(' ');
			if (parts.Length != 2)
			{
				return false;
			}

			if (reorderableListCache.TryGetValue(port.node, out var cache))
			{
				if (cache.TryGetValue(parts[0], out var list))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary> Draw an editable list of dynamic ports. Port names are named as "[fieldName] [index]" </summary>
		/// <param name = "fieldName"> Supply a list for editable values </param>
		/// <param name = "type"> Value type of added dynamic ports </param>
		/// <param name = "serializedObject"> The serializedObject of the node </param>
		/// <param name = "connectionType"> Connection type of added dynamic ports </param>
		/// <param name = "onCreation">
		///     Called on the list on creation. Use this if you want to customize the created
		///     ReorderableList
		/// </param>
		public static void DynamicPortList(string fieldName, Type type, SerializedObject serializedObject, PortDirectionType portDirectionType,
		                                   ConnectionType connectionType = ConnectionType.Multiple,
		                                   TypeConstraint typeConstraint = TypeConstraint.None,
		                                   Action<ReorderableList> onCreation = null)
		{
			var node = serializedObject.targetObject as Node;

			var indexedPorts = node.DynamicPorts.Select(
					x =>
					{
						var split = x.fieldName.Split(' ');
						if (split != null && split.Length == 2 && split[0] == fieldName)
						{
							var i = -1;
							if (int.TryParse(split[1], out i))
							{
								return new
								{
									index = i, port = x
								};
							}
						}

						return new
						{
							index = -1, port = (NodePort)null
						};
					})
				.Where(x => x.port != null);
			var dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

			node.UpdatePorts();

			ReorderableList list = null;
			if (reorderableListCache.TryGetValue(serializedObject.targetObject, out var rlc))
			{
				if (!rlc.TryGetValue(fieldName, out list))
				{
					list = null;
				}
			}

			// If a ReorderableList isn't cached for this array, do so.
			if (list == null)
			{
				var arrayData = serializedObject.FindProperty(fieldName);
				list = CreateReorderableList(
					fieldName,
					dynamicPorts,
					arrayData,
					type,
					serializedObject,
					portDirectionType,
					connectionType,
					typeConstraint,
					onCreation);
				if (reorderableListCache.TryGetValue(serializedObject.targetObject, out rlc))
				{
					rlc.Add(fieldName, list);
				}
				else
				{
					reorderableListCache.Add(
						serializedObject.targetObject,
						new Dictionary<string, ReorderableList>
						{
							{
								fieldName, list
							}
						});
				}
			}

			list.list = dynamicPorts;
			list.DoLayoutList();
		}

		private static ReorderableList CreateReorderableList(string fieldName, List<NodePort> dynamicPorts,
		                                                     SerializedProperty arrayData, Type type,
		                                                     SerializedObject serializedObject, PortDirectionType portDirectionType,
		                                                     ConnectionType connectionType,
		                                                     TypeConstraint typeConstraint,
		                                                     Action<ReorderableList> onCreation)
		{
			var hasArrayData = arrayData != null && arrayData.isArray;
			var node = serializedObject.targetObject as Node;
			var list = new ReorderableList(
				dynamicPorts,
				null,
				true,
				true,
				true,
				true);
			var label = arrayData != null ? arrayData.displayName : ObjectNames.NicifyVariableName(fieldName);

			list.drawElementCallback =
				(rect, index, isActive, isFocused) =>
				{
					var port = node.GetPort(fieldName + " " + index);
					if (hasArrayData && arrayData.propertyType != SerializedPropertyType.String)
					{
						if (arrayData.arraySize <= index)
						{
							EditorGUI.LabelField(rect, "Array[" + index + "] data out of range");
							return;
						}

						var itemData = arrayData.GetArrayElementAtIndex(index);
						EditorGUI.PropertyField(rect, itemData, true);
					}
					else
					{
						EditorGUI.LabelField(rect, port != null ? port.fieldName : "");
					}

					if (port != null)
					{
						var pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
						PortField(pos, port);
					}
				};
			list.elementHeightCallback =
				index =>
				{
					if (hasArrayData)
					{
						if (arrayData.arraySize <= index)
						{
							return EditorGUIUtility.singleLineHeight;
						}

						var itemData = arrayData.GetArrayElementAtIndex(index);
						return EditorGUI.GetPropertyHeight(itemData);
					}

					return EditorGUIUtility.singleLineHeight;
				};
			list.drawHeaderCallback =
				rect =>
				{
					EditorGUI.LabelField(rect, label);
				};
			list.onSelectCallback =
				rl =>
				{
					reorderableListIndex = rl.index;
				};
			list.onReorderCallback =
				rl =>
				{
					// Move up
					if (rl.index > reorderableListIndex)
					{
						for (var i = reorderableListIndex; i < rl.index; ++i)
						{
							var port = node.GetPort(fieldName + " " + i);
							var nextPort = node.GetPort(fieldName + " " + (i + 1));
							port.SwapConnections(nextPort);

							// Swap cached positions to mitigate twitching
							var rect = NodeEditorWindow.current.portConnectionPoints[port];
							NodeEditorWindow.current.portConnectionPoints[port] =
								NodeEditorWindow.current.portConnectionPoints[nextPort];
							NodeEditorWindow.current.portConnectionPoints[nextPort] = rect;
						}
					}
					// Move down
					else
					{
						for (var i = reorderableListIndex; i > rl.index; --i)
						{
							var port = node.GetPort(fieldName + " " + i);
							var nextPort = node.GetPort(fieldName + " " + (i - 1));
							port.SwapConnections(nextPort);

							// Swap cached positions to mitigate twitching
							var rect = NodeEditorWindow.current.portConnectionPoints[port];
							NodeEditorWindow.current.portConnectionPoints[port] =
								NodeEditorWindow.current.portConnectionPoints[nextPort];
							NodeEditorWindow.current.portConnectionPoints[nextPort] = rect;
						}
					}

					// Apply changes
					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();

					// Move array data if there is any
					if (hasArrayData)
					{
						arrayData.MoveArrayElement(reorderableListIndex, rl.index);
					}

					// Apply changes
					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();
					NodeEditorWindow.current.Repaint();
					EditorApplication.delayCall += NodeEditorWindow.current.Repaint;
				};
			list.onAddCallback =
				rl =>
				{
					// Add dynamic port postfixed with an index number
					var newName = fieldName + " 0";
					var i = 0;
					while (node.HasPort(newName))
					{
						newName = fieldName + " " + ++i;
					}

					if (portDirectionType == PortDirectionType.Output)
					{
						node.AddDynamicOutput(
							type,
							connectionType,
							TypeConstraint.None,
							newName);
					}
					else
					{
						node.AddDynamicInput(
							type,
							connectionType,
							typeConstraint,
							newName);
					}

					serializedObject.Update();
					EditorUtility.SetDirty(node);
					if (hasArrayData)
					{
						arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
					}

					serializedObject.ApplyModifiedProperties();
				};
			list.onRemoveCallback =
				rl =>
				{
					var indexedPorts = node.DynamicPorts.Select(
							x =>
							{
								var split = x.fieldName.Split(' ');
								if (split != null && split.Length == 2 && split[0] == fieldName)
								{
									var i = -1;
									if (int.TryParse(split[1], out i))
									{
										return new
										{
											index = i, port = x
										};
									}
								}

								return new
								{
									index = -1, port = (NodePort)null
								};
							})
						.Where(x => x.port != null);
					dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

					var index = rl.index;

					if (dynamicPorts[index] == null)
					{
						Debug.LogWarning("No port found at index " + index + " - Skipped");
					}
					else if (dynamicPorts.Count <= index)
					{
						Debug.LogWarning(
							"DynamicPorts[" + index + "] out of range. Length was " + dynamicPorts.Count + " - Skipped");
					}
					else
					{
						// Clear the removed ports connections
						dynamicPorts[index].ClearConnections();
						// Move following connections one step up to replace the missing connection
						for (var k = index + 1; k < dynamicPorts.Count(); k++)
						{
							for (var j = 0; j < dynamicPorts[k].ConnectionCount; j++)
							{
								var other = dynamicPorts[k].GetConnection(j);
								dynamicPorts[k].Disconnect(other);
								dynamicPorts[k - 1].Connect(other);
							}
						}

						// Remove the last dynamic port, to avoid messing up the indexing
						node.RemoveDynamicPort(dynamicPorts[dynamicPorts.Count() - 1].fieldName);
						serializedObject.Update();
						EditorUtility.SetDirty(node);
					}

					if (hasArrayData && arrayData.propertyType != SerializedPropertyType.String)
					{
						if (arrayData.arraySize <= index)
						{
							Debug.LogWarning(
								"Attempted to remove array index " +
								index +
								" where only " +
								arrayData.arraySize +
								" exist - Skipped");
							Debug.Log(rl.list[0]);
							return;
						}

						arrayData.DeleteArrayElementAtIndex(index);
						// Error handling. If the following happens too often, file a bug report at https://github.com/Siccity/xNode/issues
						if (dynamicPorts.Count <= arrayData.arraySize)
						{
							while (dynamicPorts.Count <= arrayData.arraySize)
							{
								arrayData.DeleteArrayElementAtIndex(arrayData.arraySize - 1);
							}

							Debug.LogWarning("Array size exceeded dynamic ports size. Excess items removed.");
						}

						serializedObject.ApplyModifiedProperties();
						serializedObject.Update();
					}
				};

			if (hasArrayData)
			{
				var dynamicPortCount = dynamicPorts.Count;
				while (dynamicPortCount < arrayData.arraySize)
				{
					// Add dynamic port postfixed with an index number
					var newName = arrayData.name + " 0";
					var i = 0;
					while (node.HasPort(newName))
					{
						newName = arrayData.name + " " + ++i;
					}

					if (portDirectionType == PortDirectionType.Output)
					{
						node.AddDynamicOutput(
							type,
							connectionType,
							typeConstraint,
							newName);
					}
					else
					{
						node.AddDynamicInput(
							type,
							connectionType,
							typeConstraint,
							newName);
					}

					EditorUtility.SetDirty(node);
					dynamicPortCount++;
				}

				while (arrayData.arraySize < dynamicPortCount)
				{
					arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
				}

				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

			if (onCreation != null)
			{
				onCreation(list);
			}

			return list;
		}

		#region Obsolete

		[Obsolete("Use IsDynamicPortListPort instead")]
		public static bool IsInstancePortListPort(NodePort port)
		{
			return IsDynamicPortListPort(port);
		}

		[Obsolete("Use DynamicPortList instead")]
		public static void InstancePortList(string fieldName, Type type, SerializedObject serializedObject, PortDirectionType portDirectionType,
		                                    ConnectionType connectionType = ConnectionType.Multiple,
		                                    TypeConstraint typeConstraint = TypeConstraint.None,
		                                    Action<ReorderableList> onCreation = null)
		{
			DynamicPortList(
				fieldName,
				type,
				serializedObject,
				portDirectionType,
				connectionType,
				typeConstraint,
				onCreation);
		}

		#endregion
	}
}
