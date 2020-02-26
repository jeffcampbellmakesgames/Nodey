using System;
using Examples.MathGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace Examples.RuntimeMathGraph
{
	public class UGUIContextMenu : MonoBehaviour,
	                               IPointerExitHandler
	{
		public CanvasGroup group;

		public Action<Type, Vector2> onClickSpawn;
		private Vector2 pos;

		[HideInInspector]
		public Node selectedNode;

		private void Start()
		{
			Close();
		}

		public void OpenAt(Vector2 pos)
		{
			transform.position = pos;
			group.alpha = 1;
			group.interactable = true;
			group.blocksRaycasts = true;
			transform.SetAsLastSibling();
		}

		public void Close()
		{
			group.alpha = 0;
			group.interactable = false;
			group.blocksRaycasts = false;
		}

		public void SpawnMathNode()
		{
			SpawnNode(typeof(MathNode));
		}

		public void SpawnDisplayNode()
		{
			SpawnNode(typeof(DisplayValue));
		}

		public void SpawnVectorNode()
		{
			SpawnNode(typeof(Vector));
		}

		private void SpawnNode(Type nodeType)
		{
			var pos = new Vector2(transform.localPosition.x, -transform.localPosition.y);
			onClickSpawn(nodeType, pos);
		}

		public void RemoveNode()
		{
			var runtimeMathGraph = GetComponentInParent<RuntimeMathGraph>();
			runtimeMathGraph.graph.RemoveNode(selectedNode);
			runtimeMathGraph.Refresh();
			Close();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Close();
		}
	}
}
