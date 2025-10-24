
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Knit.Framework
{
	public sealed class Slider : Selectable, IDragHandler, IInitializePotentialDragHandler
	{
		public float Value
		{
			get{ return (m_WholeNumbers != false)? Mathf.Round( m_Value) : m_Value; }
			set{ Set( value, true); }
		}
		public float MinValue
		{
			get{ return m_MinValue; }
			set{ if( m_MinValue != value){ Set( m_Value, true); UpdateVisuals(); }}
		}
		public float MaxValue
		{
			get{ return m_MaxValue; }
			set{ if( m_MaxValue != value){ Set( m_Value, true); UpdateVisuals(); }}
		}
		public bool WholeNumbers
		{
			get{ return m_WholeNumbers; }
			set{ if( m_WholeNumbers != value){ Set(m_Value, true); UpdateVisuals(); }}
		}
		public float NormalizedValue
		{
			get{ if( Mathf.Approximately( m_MinValue, m_MaxValue)){ return 0; } return Mathf.InverseLerp( m_MinValue, m_MaxValue, Value); }
			set{ Set( Mathf.Lerp( m_MinValue, m_MaxValue, value), true); }
		}
		public event UnityAction<Selectable, float> OnValueChanged
		{
			add{ m_OnValueChanged.AddListener( value); }
			remove{ m_OnValueChanged.RemoveListener( value); }
		}
	#if UNITY_EDITOR
		public event UnityAction<Selectable, float> OnPersistentValueChanged
		{
			add{ UnityEditor.Events.UnityEventTools.AddPersistentListener( m_OnValueChanged, value); }
			remove{ UnityEditor.Events.UnityEventTools.RemovePersistentListener( m_OnValueChanged, value); }
		}
	#endif
		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateCache();
			Set( m_Value, false);
			UpdateVisuals();
		}
		protected override void OnDisable()
		{
			m_Tracker.Clear();
			base.OnDisable();
		}
		public override void OnPointerDown( PointerEventData eventData)
		{
			bool isInteractable = false;
			
			if( eventData.button == PointerEventData.InputButton.Left)
			{
				isInteractable = IsInteractable();
				
				if( isInteractable != false)
				{
					m_InternalState |= InternalState.EventDown;
					
					if( HasNavigate() != false)
					{
						EventSystem.current?.SetSelectedGameObject( gameObject, eventData);
					}
					OnSetProperty();
					
					if (m_HandleContainerRect != null
					&&	RectTransformUtility.RectangleContainsScreenPoint( m_HandleRect, 
						eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera) != false)
					{
						if( RectTransformUtility.ScreenPointToLocalPointInRectangle( m_HandleRect, 
							eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localPosition) != false)
						{
							m_Offset = localPosition;
						}
					}
					else
					{
						UpdateDrag( eventData, eventData.pressEventCamera);
					}
				}
			}
			eventData.eligibleForClick = isInteractable;
		}
		public void OnDrag( PointerEventData eventData)
		{
			if( eventData.button == PointerEventData.InputButton.Left && IsInteractable() != false)
			{
				UpdateDrag( eventData, eventData.pressEventCamera);
			}
		}
		public override void OnMove(AxisEventData eventData)
		{
			if( IsInteractable() == false)
			{
				base.OnMove(eventData);
			}
			else
			{
				int axis = GetAxis();
				
				switch( eventData.moveDir)
				{
					case MoveDirection.Left:
					{
						if( (Axis)axis == Axis.Horizontal && FindSelectableOnLeft() == null)
						{
							Set( (IsReverseValue() != false)? Value + GetStepSize() : Value - GetStepSize(), true);
						}
						else
						{
							base.OnMove( eventData);
						}
						break;
					}
					case MoveDirection.Right:
					{
						if( (Axis)axis == Axis.Horizontal && FindSelectableOnRight() == null)
						{
							Set( (IsReverseValue() != false)? Value - GetStepSize() : Value + GetStepSize(), true);
						}
						else
						{
							base.OnMove(eventData);
						}
						break;
					}
					case MoveDirection.Up:
					{
						if( (Axis)axis == Axis.Vertical && FindSelectableOnUp() == null)
						{
							Set( (IsReverseValue() != false)? Value - GetStepSize() : Value + GetStepSize(), true);
						}
						else
						{
							base.OnMove(eventData);
						}
						break;
					}
					case MoveDirection.Down:
					{
						if( (Axis)axis == Axis.Vertical && FindSelectableOnDown() == null)
						{
							Set( (IsReverseValue() != false)? Value + GetStepSize() : Value - GetStepSize(), true);
						}
						else
						{
							base.OnMove(eventData);
						}
						break;
					}
				}
			}
		}
		public override Selectable FindSelectableOnUp()
		{
			if( m_NavigateOnUp.Mode == NavigateMode.Automatic && (Axis)GetAxis() == Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnUp();
		}
		public override Selectable FindSelectableOnDown()
		{
			if( m_NavigateOnDown.Mode == NavigateMode.Automatic && (Axis)GetAxis() == Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnDown();
		}
		public override Selectable FindSelectableOnLeft()
		{
			if( m_NavigateOnLeft.Mode == NavigateMode.Automatic && (Axis)GetAxis() == Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnLeft();
		}
		public override Selectable FindSelectableOnRight()
		{
			if( m_NavigateOnRight.Mode == NavigateMode.Automatic && (Axis)GetAxis() == Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnRight();
		}
		public void OnInitializePotentialDrag( PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}
		void UpdateDrag( PointerEventData eventData, Camera camera)
		{
			RectTransform clickRect = m_HandleContainerRect ?? m_FillContainerRect;
			int axis = GetAxis();
			
			if( clickRect != null && clickRect.rect.size[ axis] > 0)
			{
				Vector2 position = Vector2.zero;
				
				if( MultipleDisplayUtilities.GetRelativeMousePositionForDrag( eventData, ref position) != false
				&&	RectTransformUtility.ScreenPointToLocalPointInRectangle( 
					clickRect, position, camera, out Vector2 localCursor) != false)
				{
					localCursor -= clickRect.rect.position;
					float value = Mathf.Clamp01( (localCursor - m_Offset)[ axis] / clickRect.rect.size[ axis]);
					NormalizedValue = (IsReverseValue() != false)? 1.0f - value : value;
				}
			}
		}
		protected override void OnDidApplyAnimationProperties()
		{
			m_Value = ClampValue( m_Value);
			float oldNormalizedValue = NormalizedValue;
			
			if( m_FillContainerRect != null)
			{
				if( m_FillImage != null && m_FillImage.type == Image.Type.Filled)
				{
					oldNormalizedValue = m_FillImage.fillAmount;
				}
				else
				{
					oldNormalizedValue = (IsReverseValue() != false)?
						1.0f - m_FillRect.anchorMin[ GetAxis()] : m_FillRect.anchorMax[ GetAxis()];
				}
			}
			else if( m_HandleContainerRect != null)
			{
				oldNormalizedValue = (IsReverseValue() != false)?
					1.0f - m_HandleRect.anchorMin[ GetAxis()] : m_HandleRect.anchorMin[ GetAxis()];
			}
			UpdateVisuals();
			
			if( oldNormalizedValue != NormalizedValue)
			{
				UISystemProfilerApi.AddMarker( "Slider.value", this);
				m_OnValueChanged.Invoke( this, m_Value);
			}
			base.OnDidApplyAnimationProperties();
		}
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			
			if( isActiveAndEnabled != false)
			{
				UpdateVisuals();
			}
		}
		internal void UpdateVisuals()
		{
#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				UpdateCache();
			}
#endif
			m_Tracker.Clear();
			int axis = GetAxis();
			
			if( m_FillContainerRect != null)
			{
				m_Tracker.Add( this, m_FillRect, DrivenTransformProperties.Anchors);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;
				
				if( m_FillImage != null && m_FillImage.type == Image.Type.Filled)
				{
					m_FillImage.fillAmount = NormalizedValue;
				}
				else if( IsReverseValue() != false)
				{
					anchorMin[ axis] = 1.0f - NormalizedValue;
				}
				else
				{
					anchorMax[ axis] = NormalizedValue;
				}
				m_FillRect.anchorMin = anchorMin;
				m_FillRect.anchorMax = anchorMax;
			}
			if( m_HandleContainerRect != null)
			{
				m_Tracker.Add( this, m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;
				anchorMin[ axis] = anchorMax[ axis] = (IsReverseValue() != false)? (1.0f - NormalizedValue) : NormalizedValue;
				m_HandleRect.anchorMin = anchorMin;
				m_HandleRect.anchorMax = anchorMax;
			}
		}
		internal void SetDirection( Direction direction)
		{
			Axis oldAxis = (Axis)GetAxis();
			bool oldReverse = IsReverseValue();
			m_Direction = direction;
			
			if( (Axis)GetAxis() != oldAxis)
			{
				RectTransformUtility.FlipLayoutAxes( transform as RectTransform, true, true);
			}
			if( IsReverseValue() != oldReverse)
			{
				RectTransformUtility.FlipLayoutOnAxis( transform as RectTransform, GetAxis(), true, true);
			}
		}
		void UpdateCache()
		{
			if( m_FillRect != null && m_FillRect != (RectTransform)transform)
			{
				m_FillImage = m_FillRect.GetComponent<Image>();
				Transform fillTransform = m_FillRect.transform;
				m_FillContainerRect = fillTransform.parent?.GetComponent<RectTransform>();
			}
			else
			{
				m_FillImage = null;
				m_FillRect = null;
				m_FillContainerRect = null;
			}
			if( m_HandleRect && m_HandleRect != (RectTransform)transform)
			{
				Transform handleTransform = m_HandleRect.transform;
				m_HandleContainerRect = handleTransform.parent?.GetComponent<RectTransform>();
			}
			else
			{
				m_HandleRect = null;
				m_HandleContainerRect = null;
			}
		}
		void Set( float value, bool sendCallback)
		{
			value = ClampValue( value);
			
			if( m_Value != value)
			{
				m_Value = value;
				UpdateVisuals();
			}
			if( sendCallback != false)
			{
				UISystemProfilerApi.AddMarker( "Slider.value", this);
				m_OnValueChanged.Invoke( this, value);
			}
		}
		float ClampValue( float value)
		{
			value = Mathf.Clamp( value, m_MinValue, m_MaxValue);
			
			if( m_WholeNumbers != false)
			{
				value = Mathf.Round( value);
			}
			return value;
		}
		bool IsReverseValue()
		{
			return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom;
		}
		int GetAxis()
		{
			return (int)m_Direction / 2;
		}
		float GetStepSize()
		{
			return (m_WholeNumbers != false)? 1.0f : (m_MaxValue - m_MinValue) * 0.1f;
		}
	#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			Graphic = m_HandleRect.GetComponent<Image>();
		}
		internal void SetFromEditor( float value)
		{
			m_Value = ClampValue( value);
			UpdateVisuals();
			
			if( Application.isPlaying != false)
			{
				UISystemProfilerApi.AddMarker( "Slider.value", this);
				m_OnValueChanged.Invoke( this, value);
			}
		}
	#endif
		internal enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom,
		}
		enum Axis
		{
			Horizontal,
			Vertical
		}
		[System.Serializable]
		sealed class SliderEvent : UnityEvent<Selectable, float>{}
		[SerializeField]
		Direction m_Direction = Direction.LeftToRight;
		[SerializeField]
		internal RectTransform m_FillRect;
		[SerializeField]
		internal RectTransform m_HandleRect;
		[SerializeField]
		float m_Value;
		[SerializeField]
		float m_MinValue;
		[SerializeField]
		float m_MaxValue = 1;
		[SerializeField]
		bool m_WholeNumbers;
		[SerializeField]
		SliderEvent m_OnValueChanged = new();
		
		Image m_FillImage;
		Vector2 m_Offset = Vector2.zero;
		RectTransform m_FillContainerRect;
		RectTransform m_HandleContainerRect;
		DrivenRectTransformTracker m_Tracker;
	}
}
