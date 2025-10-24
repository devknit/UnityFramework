
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace Knit.Framework
{
	public class Button : Selectable, ISubmitHandler
	{
		public event UnityAction<Selectable> OnClick
		{
			add{ m_OnClick.AddListener( value); }
			remove{ m_OnClick.RemoveListener( value); }
		}
		public event UnityAction<Selectable> OnLongPress
		{
			add{ m_OnLongPress.AddListener( value); }
			remove{ m_OnLongPress.RemoveListener( value); }
		}
	#if UNITY_EDITOR
		public event UnityAction<Selectable> OnPersistentClick
		{
			add{ UnityEditor.Events.UnityEventTools.AddPersistentListener( m_OnClick, value); }
			remove{ UnityEditor.Events.UnityEventTools.RemovePersistentListener( m_OnClick, value); }
		}
		public event UnityAction<Selectable> OnPersistentLongPress
		{
			add{ UnityEditor.Events.UnityEventTools.AddPersistentListener( m_OnLongPress, value); }
			remove{ UnityEditor.Events.UnityEventTools.RemovePersistentListener( m_OnLongPress, value); }
		}
	#endif
		protected override void OnDisable()
		{
			base.OnDisable();
			StopCoroutines();
		}
		void OnApplicationFocus( bool hasFocus)
		{
			if( hasFocus == false)
			{
				StopCoroutines();
			}
		}
		public override void OnPointerDown( PointerEventData eventData)
		{
			bool isInteractable = false;
			
			if( eventData.button == PointerEventData.InputButton.Left)
			{
				m_UseClickEvent = false;
				StopCoroutines();
				
				isInteractable = IsInteractable();
				
				if( isInteractable != false)
				{
					m_InternalState |= InternalState.EventDown;
					
					if( HasNavigate() != false)
					{
						EventSystem.current?.SetSelectedGameObject( gameObject, eventData);
					}
					OnSetProperty();
					
					if( (m_ClickMode & ClickMode.RepeatClick) != 0)
					{
						float startSeconds = 0.6f;
						float intervalSeconds = 0.001f;
						
						if( startSeconds > 0.0f && intervalSeconds > 0.0f)
						{
							m_RepeatClickCoroutine = StartCoroutine( 
								RepeatClickProcess( startSeconds, intervalSeconds));
						}
					}
					if( (m_ClickMode & ClickMode.LongPress) != 0)
					{
						int? fingerId = null;
						
					#if UNITY_IOS
						if( Input.touchPressureSupported != false)
						{
							fingerId = eventData.pointerId;
						}
					#endif
						float seconds = 1.0f;
						if( seconds > 0.0f)
						{
							m_LongPressCoroutine = StartCoroutine( 
								LongPressProcess( seconds, fingerId));
						}
					}
				}
			}
			eventData.eligibleForClick = isInteractable;
		}
		public override void OnPointerUp( PointerEventData eventData)
		{
			if( eventData.button == PointerEventData.InputButton.Left)
			{
				m_InternalState &= ~InternalState.EventDown;
				
				if( m_UseClickEvent != false)
				{
					eventData.Use();
				}
				StopCoroutines();
				
				if( ((m_InternalState & (InternalState.EventDown | InternalState.EventEnter)) != 0)
				&&	eventData.used == false && eventData.dragging == false && eventData.eligibleForClick != false)
				{
					OnSubmit( eventData);
				}
				else
				{
					OnSetProperty();
				}
			}
		}
		public override void OnSubmit( BaseEventData eventData)
		{
			m_InternalState |= InternalState.EventSubmit;
			OnSetProperty();
			
			if( (m_ClickMode & ClickMode.PrecedeEvent) != 0)
			{
				m_OnClick.Invoke( this);
			}
		}
		private protected override void OnActionCompleted( SelectionState state)
		{
			if( state == SelectionState.Submited)
			{
				if( (m_ClickMode & ClickMode.PrecedeEvent) == 0)
				{
					m_OnClick.Invoke( this);
				}
				m_InternalState &= ~InternalState.EventSubmit;
				OnSetProperty();
			}
		}
		IEnumerator LongPressProcess( float duration, int? fingerId)
		{
			if( fingerId.HasValue == false)
			{
				yield return (m_IgnoreTimeScale == false)?
					new WaitForSeconds( duration) :
					new WaitForSecondsRealtime( duration);
			}
			else
			{
				bool longPressed = false;
				
				while( duration > 0.0f && longPressed == false)
				{
					yield return null;
					
					duration -= (m_IgnoreTimeScale == false)? Time.deltaTime : Time.unscaledDeltaTime;
					Touch[] touches = Input.touches;
					
					for( int i0 = 0; i0 < touches.Length; ++i0)
					{
						if( touches[ i0].fingerId == fingerId.Value)
						{
							if( touches[ i0].pressure >= 1.0f)
							{
								longPressed = true;
							}
							break;
						}
					}
				}
			}
			m_UseClickEvent = true;
			m_OnLongPress.Invoke( this);
			// Vibration.Play();
		}
		IEnumerator RepeatClickProcess( float startSeconds, float invervalSeconds)
		{
			yield return (m_IgnoreTimeScale == false)?
				new WaitForSeconds( startSeconds) :
				new WaitForSecondsRealtime( startSeconds);
			
			m_UseClickEvent = true;
			do
			{
				m_OnClick.Invoke( this);
				yield return (m_IgnoreTimeScale == false)?
					new WaitForSeconds( invervalSeconds) :
					new WaitForSecondsRealtime( invervalSeconds);
			}
			while( true);
		}
		void StopCoroutines()
		{
			if( m_LongPressCoroutine != null)
			{
				StopCoroutine( m_LongPressCoroutine);
				m_LongPressCoroutine = null;
			}
			if( m_RepeatClickCoroutine != null)
			{
				StopCoroutine( m_RepeatClickCoroutine);
				m_RepeatClickCoroutine = null;
			}
		}
		enum ClickMode
		{
			None = 0,
			RepeatClick = 1 << 0,
			LongPress = 1 << 1,
			PrecedeEvent = 1 << 2,
			[InspectorName( "RepeatClick & PrecedeEvent")]
			RepeatClickAndPrecedeEvent = RepeatClick | PrecedeEvent,
			[InspectorName( "LongPress & PrecedeEvent")]
			LongPressAndPrecedeEvent = LongPress | PrecedeEvent,
		}
		[System.Serializable]
		sealed class ButtonEvent : UnityEvent<Selectable>{}
		[SerializeField]
		ClickMode m_ClickMode = ClickMode.None;
		[SerializeField]
		ButtonEvent m_OnClick = new();
		[SerializeField]
		ButtonEvent m_OnLongPress = new();
		
		[System.NonSerialized]
		Coroutine m_LongPressCoroutine;
		[System.NonSerialized]
		Coroutine m_RepeatClickCoroutine;
		[System.NonSerialized]
		bool m_UseClickEvent;
	}
}
