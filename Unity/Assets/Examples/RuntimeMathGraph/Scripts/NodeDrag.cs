using UnityEngine;
using UnityEngine.EventSystems;

namespace Examples.RuntimeMathGraph
{
	public class NodeDrag : MonoBehaviour,
	                        IPointerClickHandler,
	                        IBeginDragHandler,
	                        IDragHandler,
	                        IEndDragHandler
	{
		private UGUIMathBaseNode node;
		private Vector3 offset;

		private void Awake()
		{
			node = GetComponentInParent<UGUIMathBaseNode>();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			Vector2 pointer = node.graph.scrollRect.content.InverseTransformPoint(eventData.position);
			Vector2 pos = node.transform.localPosition;
			offset = pointer - pos;
		}

		public void OnDrag(PointerEventData eventData)
		{
			node.transform.localPosition = node.graph.scrollRect.content.InverseTransformPoint(eventData.position) - offset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			node.transform.localPosition = node.graph.scrollRect.content.InverseTransformPoint(eventData.position) - offset;
			Vector2 pos = node.transform.localPosition;
			pos.y = -pos.y;
			node.node.position = pos;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Right)
			{
				return;
			}

			node.graph.nodeContextMenu.selectedNode = node.node;
			node.graph.nodeContextMenu.OpenAt(eventData.position);
		}
	}
}
