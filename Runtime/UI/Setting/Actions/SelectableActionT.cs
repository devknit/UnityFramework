
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Knit.Framework
{
	public abstract class SelectableActionT<TStateAction, TActionContext> : SelectableAction
		where TStateAction : SelectableStateActionT<TActionContext>
		where TActionContext : SelectableActionContext
	{
		internal override void SetValue( SelectionState state, Graphic graphic, Transform transform)
		{
			TStateAction action = state switch
			{
				SelectionState.Normal => m_Normal,
				SelectionState.Highlighted => m_Highlighte,
				SelectionState.Pressed => m_Presse,
				SelectionState.Selected => m_Select,
				SelectionState.Disabled => m_Disable,
				SelectionState.Submited => m_Submit,
				_ => null
			};
			action?.OnSetValue( action.CreateContext( graphic, transform), 1.0f);
		}
		internal override IEnumerator OnTween( 
			SelectionState state, Graphic graphic, Transform transform, 
			bool ignoreTimeScale, System.Action<SelectionState> onActionCompleted)
		{
			TStateAction action = state switch
			{
				SelectionState.Normal => m_Normal,
				SelectionState.Highlighted => m_Highlighte,
				SelectionState.Pressed => m_Presse,
				SelectionState.Selected => m_Select,
				SelectionState.Disabled => m_Disable,
				SelectionState.Submited => m_Submit,
				_ => null
			};
			if( action != null)
			{
				TActionContext context = action.CreateContext( graphic, transform);
				float duration = action.Duration;
				float elapsedTime = 0.0f;
				float deltaTime;
				
				while( elapsedTime < duration)
				{
					deltaTime = (ignoreTimeScale != false)? Time.unscaledDeltaTime : Time.deltaTime;
					
					if( deltaTime >= duration)
					{
						deltaTime = duration * 0.5f;
					}
					elapsedTime += deltaTime;
					
					if( elapsedTime >= duration)
					{
						break;
					}
					action.OnSetValue( context, Mathf.Clamp01( elapsedTime / duration));
					yield return null;
				}
				action.OnSetValue( context, 1.0f);
			}
			onActionCompleted?.Invoke( state);
		}
		[SerializeField]
		TStateAction m_Disable;
		[SerializeField]
		TStateAction m_Normal;
		[SerializeField]
		TStateAction m_Highlighte;
		[SerializeField]
		TStateAction m_Select;
		[SerializeField]
		TStateAction m_Presse;
		[SerializeField]
		TStateAction m_Submit;
	}
}
