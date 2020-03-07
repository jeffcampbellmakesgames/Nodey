using System.IO;
using UnityEditor;
using UnityEngine;

namespace JCMG.Nodey.Editor
{
	/// <summary> Deals with modified assets </summary>
	internal class NodeEditorAssetModProcessor : UnityEditor.AssetModificationProcessor
	{
		/// <summary>
		///     Automatically delete Node sub-assets before deleting their script. This is important to do, because you can't
		///     delete null sub assets.
		///     <para/>
		///     For another workaround, see: https://gitlab.com/RotaryHeart-UnityShare/subassetmissingscriptdelete
		/// </summary>
		private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
		{
			// Skip processing anything without the .cs extension
			if (Path.GetExtension(path) != ".cs")
			{
				return AssetDeleteResult.DidNotDelete;
			}

			// Get the object that is requested for deletion
			var obj = AssetDatabase.LoadAssetAtPath<Object>(path);

			// If we aren't deleting a script, return
			if (!(obj is MonoScript))
			{
				return AssetDeleteResult.DidNotDelete;
			}

			// Check script type. Return if deleting a non-node script
			var script = obj as MonoScript;
			var scriptType = script.GetClass();
			if (scriptType == null || scriptType != typeof(Node) && !scriptType.IsSubclassOf(typeof(Node)))
			{
				return AssetDeleteResult.DidNotDelete;
			}

			// Find all ScriptableObjects using this script
			var guids = AssetDatabase.FindAssets("t:" + scriptType);
			for (var i = 0; i < guids.Length; i++)
			{
				var assetpath = AssetDatabase.GUIDToAssetPath(guids[i]);
				var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetpath);
				for (var k = 0; k < objs.Length; k++)
				{
					var node = objs[k] as Node;
					if (node.GetType() == scriptType)
					{
						if (node != null && node.graph != null)
						{
							// Delete the node and notify the user
							Debug.LogWarning(
								node.name +
								" of " +
								node.graph +
								" depended on deleted script and has been removed automatically.",
								node.graph);
							node.graph.RemoveNode(node);
						}
					}
				}
			}

			// We didn't actually delete the script. Tell the internal system to carry on with normal deletion procedure
			return AssetDeleteResult.DidNotDelete;
		}

		/// <summary> Automatically re-add loose node assets to the Graph node list </summary>
		[InitializeOnLoadMethod]
		private static void OnReloadEditor()
		{
			// Find all NodeGraph assets
			var guids = AssetDatabase.FindAssets("t:" + typeof(NodeGraph));
			for (var i = 0; i < guids.Length; i++)
			{
				var assetpath = AssetDatabase.GUIDToAssetPath(guids[i]);
				var graph = AssetDatabase.LoadAssetAtPath(assetpath, typeof(NodeGraph)) as NodeGraph;
				graph.nodes.RemoveAll(x => x == null); //Remove null items
				var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetpath);
				// Ensure that all sub node assets are present in the graph node list
				for (var u = 0; u < objs.Length; u++)
				{
					// Ignore null sub assets
					if (objs[u] == null)
					{
						continue;
					}

					if (!graph.nodes.Contains(objs[u] as Node))
					{
						graph.nodes.Add(objs[u] as Node);
					}
				}
			}
		}
	}
}
