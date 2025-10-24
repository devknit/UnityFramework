
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Knit.Framework
{
	[Serializable]
	sealed class SelectableActionColor : SelectableActionT<SelectableStateActionColor, SelectableActionColorContext>
	{
		protected override string GetDefaultActionName()
		{
			return "Color"; 
		}
    }
	sealed class SelectableActionColorContext : SelectableActionContext
	{
		public Color color;
	}
	[Serializable]
	sealed class SelectableStateActionColor : SelectableStateActionT<SelectableActionColorContext>
	{
		protected internal override SelectableActionColorContext CreateContext( Graphic graphic, Transform transform)
		{
			return new SelectableActionColorContext
			{
				graphic = graphic,
				transform = transform,
				color = graphic.canvasRenderer.GetColor(),
			};
		}
		protected internal override void OnSetValue( SelectableActionColorContext context, float t)
		{
			t = (m_BlendTime <= 0)? 1.0f : Mathf.Clamp01( t / m_BlendTime);
			context.graphic.canvasRenderer.SetColor( Color.Lerp( context.color, m_Color, t));
		}
		[SerializeField, Range( 0, 1)]
		float m_BlendTime;
		[SerializeField, ColorUsage( true, true)]
		Color m_Color = Color.white;
	}
}
