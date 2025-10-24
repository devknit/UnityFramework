
using UnityEngine;
using UnityEngine.UI;

namespace Knit.Framework
{
	[System.Serializable]
	public abstract class SelectableStateActionT<TActionContext>
		where TActionContext : SelectableActionContext
	{
		protected internal virtual TActionContext CreateContext( Graphic graphic, Transform transform)
		{
			return null; // new SelectableActionContext{ graphic = graphic, transform = transform };
		}
		protected internal virtual void OnSetValue( TActionContext context, float t)
		{
		}
		public float Duration
		{
			get{ return m_Duration; }
		}
		[SerializeField]
		float m_Duration;
	}
}
