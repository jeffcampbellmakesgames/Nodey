using System;
using System.Collections.Generic;
using System.Linq;
using JCMG.Nodey;
using JCMG.Nodey.Editor;
using UnityEditor;
using UnityEngine;

namespace Examples.LogicToy.Editor
{
	[CustomNodeGraphEditor(typeof(LogicGraph))]
	public class LogicGraphEditor : NodeGraphEditor
	{
		/// <summary> Used for tracking when an arbitrary object was last 'on' for fading effects </summary>
		private class ObjectLastOnTimer
		{
			public double lastOnTime;
			public readonly object obj;

			public ObjectLastOnTimer(object obj, bool on)
			{
				this.obj = obj;
			}
		}

		private readonly Color boolColor = new Color(0.1f, 0.6f, 0.6f);
		private double lastFrame;
		private readonly List<ObjectLastOnTimer> lastOnTimers = new List<ObjectLastOnTimer>();

		/// <summary>
		///     Overriding GetNodeMenuName lets you control if and how nodes are categorized. In this example we are sorting
		///     out all node types that are not in the XNode.Examples namespace.
		/// </summary>
		public override string GetNodeMenuName(Type type)
		{
			if (type.Namespace == "Examples.LogicToy")
			{
				return base.GetNodeMenuName(type).Replace("X Node/Examples/Logic Toy/", "");
			}

			return null;
		}

		public override void OnGUI()
		{
			// Repaint each frame
			window.Repaint();

			// Timer
			if (Event.current.type == EventType.Repaint)
			{
				for (var i = 0; i < target.nodes.Count; i++)
				{
					var timerTick = target.nodes[i] as ITimerTick;
					if (timerTick != null)
					{
						var deltaTime = (float)(EditorApplication.timeSinceStartup - lastFrame);
						timerTick.Tick(deltaTime);
					}
				}
			}

			lastFrame = EditorApplication.timeSinceStartup;
		}

		/// <summary> Controls graph noodle colors </summary>
		public override Gradient GetNoodleGradient(NodePort output, NodePort input)
		{
			var node = output.node as LogicNode;
			var baseGradient = base.GetNoodleGradient(output, input);
			HighlightGradient(
				baseGradient,
				Color.yellow,
				output,
				(bool)node.GetValue(output));
			return baseGradient;
		}

		/// <summary> Controls graph type colors </summary>
		public override Color GetTypeColor(Type type)
		{
			if (type == typeof(bool))
			{
				return boolColor;
			}

			return GetTypeColor(type);
		}

		/// <summary> Returns the time at which an arbitrary object was last 'on' </summary>
		public double GetLastOnTime(object obj, bool high)
		{
			var timer = lastOnTimers.FirstOrDefault(x => x.obj == obj);
			if (timer == null)
			{
				timer = new ObjectLastOnTimer(obj, high);
				lastOnTimers.Add(timer);
			}

			if (high)
			{
				timer.lastOnTime = EditorApplication.timeSinceStartup;
			}

			return timer.lastOnTime;
		}

		/// <summary> Returns a color based on if or when an arbitrary object was last 'on' </summary>
		public Color GetLerpColor(Color off, Color on, object obj, bool high)
		{
			var lastOnTime = GetLastOnTime(obj, high);

			if (high)
			{
				return on;
			}

			var t = (float)(lastOnTime - EditorApplication.timeSinceStartup);
			t *= 8f;
			if (t > 0)
			{
				return Color.Lerp(off, on, t);
			}

			return off;
		}

		/// <summary> Returns a color based on if or when an arbitrary object was last 'on' </summary>
		public void HighlightGradient(Gradient gradient, Color highlightColor, object obj, bool high)
		{
			var lastOnTime = GetLastOnTime(obj, high);
			float t;

			if (high)
			{
				t = 1f;
			}
			else
			{
				t = (float)(lastOnTime - EditorApplication.timeSinceStartup);
				t *= 8f;
				t += 1;
			}

			t = Mathf.Clamp01(t);
			var colorKeys = gradient.colorKeys;
			for (var i = 0; i < colorKeys.Length; i++)
			{
				var colorKey = colorKeys[i];
				colorKey.color = Color.Lerp(colorKeys[i].color, highlightColor, t);
				colorKeys[i] = colorKey;
			}

			gradient.SetKeys(colorKeys, gradient.alphaKeys);
		}
	}
}
