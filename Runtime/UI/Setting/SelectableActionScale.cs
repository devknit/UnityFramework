
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Knit.Framework
{
	[Serializable]
	sealed class SelectableActionScale : SelectableActionT<SelectableStateActionScale, SelectableActionScaleContext>
	{
		protected override string GetDefaultActionName()
		{
			return "Scale"; 
		}
    }
	sealed class SelectableActionScaleContext : SelectableActionContext
	{
		public Vector3 localScale;
	}
	[Serializable]
	sealed class SelectableStateActionScale : SelectableStateActionT<SelectableActionScaleContext>
	{
		protected internal override SelectableActionScaleContext CreateContext( Graphic graphic, Transform transform)
		{
			return new SelectableActionScaleContext
			{
				graphic = graphic,
				transform = graphic?.transform,
				localScale = graphic?.transform.localScale ?? Vector3.one
			};
		}
		protected internal override void OnSetValue( SelectableActionScaleContext context, float t)
		{
			if( context.transform is RectTransform rectTransform)
			{
				float b = (m_BlendTime <= 0)? 1.0f : Mathf.Clamp01( t / m_BlendTime);
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
		}
		[SerializeField, Range( 0, 1)]
		float m_BlendTime;
		[SerializeField]
		AnimationCurve m_Curve;
	}
}
