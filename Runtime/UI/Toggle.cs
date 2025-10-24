
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Knit.Framework
{
	public class Toggle : Selectable, IPointerClickHandler, ISubmitHandler
	{
		public bool IsOn
		{
			get{ return m_IsOn; }
		}
		public Graphic ToggleGraphic
		{
			get { return m_ToggleGraphic; }
			set { m_ToggleGraphic = value; }
		}
		public ToggleGroup ToggleGroup
		{
			get{ return m_ToggleGroup; }
			set{ SetToggleGroup( value, true); }
		}
		public event UnityAction<Selectable, bool> OnValueChanged
		{
			add{ m_OnValueChanged.AddListener( value); }
			remove{ m_OnValueChanged.RemoveListener( value); }
		}
	#if UNITY_EDITOR
		public event UnityAction<Selectable, bool> OnPersistentValueChanged
		{
			add{ UnityEditor.Events.UnityEventTools.AddPersistentListener( m_OnValueChanged, value); }
			remove{ UnityEditor.Events.UnityEventTools.RemovePersistentListener( m_OnValueChanged, value); }
		}
	#endif
		public void Set( bool isOn)
		{
			if( m_IsOn != isOn)
			{
				m_IsOn = isOn;
				ApplyToggle();
			}
		}
		public void SetWithoutGroup( bool isOn, bool sendCallback)
		{
			if( m_IsOn != isOn)
			{
				m_IsOn = isOn;
				ApplyGraphic();
				
				if( sendCallback != false)
				{
					m_OnValueChanged.Invoke( this, m_IsOn);
				}
			}
		}
		public void OnPointerClick( PointerEventData eventData)
		{
			if( eventData.button == PointerEventData.InputButton.Left)
			{
				OnSubmit( eventData);
			}
		}
		public override void OnSubmit( BaseEventData eventData)
		{
			if( m_ToggleGroup == null)
			{
				m_InternalState |= InternalState.EventSubmit;
				OnSetProperty();
				Set( !m_IsOn);
			}
			else if( m_IsOn == false)
			{
				m_InternalState |= InternalState.EventSubmit;
				OnSetProperty();
				Set( true);
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			ApplyGraphic();
			
			if( m_ToggleGroup != null)
			{
				SetToggleGroup( m_ToggleGroup, false);
			}
			else if( m_AwakeOnEvent != false)
			{
				m_OnValueChanged.Invoke( this, m_IsOn);
			}
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			SetToggleGroup( null, false);
		}
		internal void ApplyToggle()
		{
			ApplyGraphic();
			m_ToggleGroup?.Normalize( this, true);
			m_OnValueChanged.Invoke( this, m_IsOn);
		}
		void ApplyGraphic()
		{
			if( m_ToggleGraphic != null)
			{
				if( m_OverrideSprite != null && m_ToggleGraphic is Image image)
				{
					image.overrideSprite = (m_IsOn != false)? m_OverrideSprite : null;
					m_ToggleGraphic.canvasRenderer.SetAlpha( 1.0f);
				}
				else
				{
					m_ToggleGraphic.canvasRenderer.SetAlpha( (m_IsOn != false)? 1.0f : 0.0f);
				}
			}
		}
		void SetToggleGroup( ToggleGroup group, bool overrideGroup)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				if( overrideGroup != false)
				{
					m_ToggleGroup = group;
				}
			}
		#endif
			if( m_RegisterToggle != false || (overrideGroup != false && m_ToggleGroup != group))
			{
				m_ToggleGroup?.UnregisterToggle( this);
				m_RegisterToggle = false;
			}
			if( overrideGroup != false)
			{
				m_ToggleGroup = group;
			}
			if( isActiveAndEnabled != false)
			{
				m_RegisterToggle = group?.RegisterToggle( this) ?? false;
				
				if( group.Normalize( this, false) != false)
				{
					m_OnValueChanged.Invoke( this, m_IsOn);
				}
			}
		}
		[System.Serializable]
		sealed class ToggleEvent : UnityEvent<Selectable, bool>{}
		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs( "m_TargetGraphic")]
		Graphic m_ToggleGraphic;
		[SerializeField]
		Sprite m_OverrideSprite;
		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs( "m_ToggleGroup")]
		internal ToggleGroup m_ToggleGroup;
		[SerializeField]
		bool m_AwakeOnEvent;
		[SerializeField]
		internal bool m_IsOn;
		[SerializeField]
		ToggleEvent m_OnValueChanged = new();
		[System.NonSerialized]
		bool m_RegisterToggle;
	}
}
