using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

namespace Examples.RuntimeMathGraph
{
	public class UGUIMathBaseNode : MonoBehaviour,
	                                IDragHandler
	{
		[HideInInspector]
		public RuntimeMathGraph graph;

		public Text header;

		[HideInInspector]
		public Node node;

		private UGUIPort[] ports;

		public virtual void Start()
		{
			ports = GetComponentsInChildren<UGUIPort>();
			foreach (var port in ports)
			{
				port.node = node;
			}

			header.text = node.name;
			SetPosition(node.position);
		}

		public virtual void UpdateGUI()
		{
		}

		private void LateUpdate()
		{
			foreach (var port in ports)
			{
				port.UpdateConnectionTransforms();
			}
		}

		public UGUIPort GetPort(string name)
		{
			for (var i = 0; i < ports.Length; i++)
			{
				if (ports[i].name == name)
				{
					return ports[i];
				}
			}

			return null;
		}

		public void SetPosition(Vector2 pos)
		{
			pos.y = -pos.y;
			transform.localPosition = pos;
		}

		public void SetName(string name)
		{
			header.text = name;
		}

		public void OnDrag(PointerEventData eventData)
		{
		}
	}
}
