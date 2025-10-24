
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Knit.Framework
{
	[Serializable]
	sealed class SelectableActionColorScale : SelectableActionT<SelectableStateActionColorScale, SelectableActionColorScaleContext>
	{
		protected override string GetDefaultActionName()
		{
			return "ColorScale"; 
		}
    }
	sealed class SelectableActionColorScaleContext : SelectableActionContext
	{
		public Color color;
		public Vector3 localScale;
	}
	[Serializable]
	sealed class SelectableStateActionColorScale : SelectableStateActionT<SelectableActionColorScaleContext>
	{
		protected internal override SelectableActionColorScaleContext CreateContext( Graphic graphic, Transform transform)
		{
			return new SelectableActionColorScaleContext
			{
				graphic = graphic,
				transform = transform,
				color = graphic.canvasRenderer.GetColor(),
				localScale = transform.localScale
			};
		}
		protected internal override void OnSetValue( SelectableActionColorScaleContext context, float t)
		{
			float b = (m_BlendTime <= 0)? 1.0f : Mathf.Clamp01( t / m_BlendTime);
			
			if( context.transform is RectTransform rectTransform)
			{
				float value = m_Curve.Evaluate( t);
				Vector3 localScale = Vector3.Lerp( 
					context.localScale, 
					new Vector3( value, value, 1.0f), b);
				Vector2 sizeDelta = rectTransform.sizeDelta;
				float widthOffset = ((sizeDelta.x * localScale.x) - sizeDelta.x) * 0.5f;
				float heightOffset = ((sizeDelta.y * localScale.y) - sizeDelta.y) * 0.5f;
				context.graphic.raycastPadding = new Vector4( 
					widthOffset, heightOffset, widthOffset, heightOffset);
				rectTransform.localScale = localScale;
			}
			context.graphic.canvasRenderer.SetColor( Color.Lerp( context.color, m_Color, t));
		}
		[SerializeField, Range( 0, 1)]
		float m_BlendTime;
		[SerializeField, ColorUsage( true, true)]
		Color m_Color = Color.white;
		[SerializeField]
		AnimationCurve m_Curve;
	}
}
