using UnityEngine;
using UnityEngine.UI;

namespace Examples.RuntimeMathGraph
{
	public class UGUITooltip : MonoBehaviour
	{
		private RuntimeMathGraph graph;
		public CanvasGroup group;
		public Text label;
		private bool show;

		private void Awake()
		{
			graph = GetComponentInParent<RuntimeMathGraph>();
		}

		private void Start()
		{
			Hide();
		}

		private void Update()
		{
			if (show)
			{
				UpdatePosition();
			}
		}

		public void Show()
		{
			show = true;
			group.alpha = 1;
			UpdatePosition();
			transform.SetAsLastSibling();
		}

		public void Hide()
		{
			show = false;
			group.alpha = 0;
		}

		private void UpdatePosition()
		{
			var rect = graph.scrollRect.content.transform as RectTransform;
			var cam = graph.gameObject.GetComponentInParent<Canvas>().worldCamera;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				rect,
				Input.mousePosition,
				cam,
				out var pos);
			transform.localPosition = pos;
		}
	}
}
