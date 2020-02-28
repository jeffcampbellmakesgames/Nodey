using Examples.MathGraph;
using UnityEngine.UI;

namespace Examples.RuntimeMathGraph
{
	public class UGUIDisplayValue : UGUIMathBaseNode
	{
		public Text label;

		private void Update()
		{
			var displayValue = node as DisplayValue;
			var obj = displayValue.GetInputValue<object>("input");
			if (obj != null)
			{
				label.text = obj.ToString();
			}
			else
			{
				label.text = "n/a";
			}
		}
	}
}
