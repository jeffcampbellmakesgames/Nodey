using Examples.MathGraph;
using UnityEngine.UI;

namespace Examples.RuntimeMathGraph
{
	public class UGUIVector : UGUIMathBaseNode
	{
		public InputField valX;
		public InputField valY;
		public InputField valZ;

		private Vector vectorNode;

		public override void Start()
		{
			base.Start();
			vectorNode = node as Vector;

			valX.onValueChanged.AddListener(OnChangeValX);
			valY.onValueChanged.AddListener(OnChangeValY);
			valZ.onValueChanged.AddListener(OnChangeValZ);
			UpdateGUI();
		}

		public override void UpdateGUI()
		{
			var portX = node.GetInputPort("x");
			var portY = node.GetInputPort("y");
			var portZ = node.GetInputPort("z");
			valX.gameObject.SetActive(!portX.IsConnected);
			valY.gameObject.SetActive(!portY.IsConnected);
			valZ.gameObject.SetActive(!portZ.IsConnected);

			var vectorNode = node as Vector;
			valX.text = vectorNode.x.ToString();
			valY.text = vectorNode.y.ToString();
			valZ.text = vectorNode.z.ToString();
		}

		private void OnChangeValX(string val)
		{
			vectorNode.x = float.Parse(valX.text);
		}

		private void OnChangeValY(string val)
		{
			vectorNode.y = float.Parse(valY.text);
		}

		private void OnChangeValZ(string val)
		{
			vectorNode.z = float.Parse(valZ.text);
		}
	}
}
