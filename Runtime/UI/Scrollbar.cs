
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Knit.Framework
{
	[RequireComponent( typeof( RectTransform))]
	public class Scrollbar : Selectable, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		public enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom,
		}
		enum Axis
		{
			Horizontal = 0,
			Vertical = 1
		}
		public RectTransform HandleRect
		{
			get{ return m_HandleRect; }
			set{ if( SetPropertyUtility.SetClass( ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } }
		}
		public int NumberOfSteps
		{
			get{ return m_NumberOfSteps; }
			set{ if (SetPropertyUtility.SetStruct( ref m_NumberOfSteps, value)) { Set( m_Value); UpdateVisuals(); } }
		}
		public Direction ScrollDirection
		{
			get { return m_Direction; }
			set { if (SetPropertyUtility.SetStruct( ref m_Direction, value)) UpdateVisuals(); }
		}
		public float Size
		{
			get{ return m_Size; }
			set { if( SetPropertyUtility.SetStruct( ref m_Size, Mathf.Clamp01(value))) UpdateVisuals(); }
		}
		public float Value
		{
			get
			{
				float val = m_Value;
				
				if( m_NumberOfSteps > 1)
				{
					val = Mathf.Round( val * (m_NumberOfSteps - 1)) / (m_NumberOfSteps - 1);
				}
				return val;
			}
			set
			{
				Set( value);
			}
		}
		public ScrollEvent OnValueChanged
		{
			get { return m_OnValueChanged; }
			set { m_OnValueChanged = value; }
		}
		float StepSize
		{
			get { return (m_NumberOfSteps > 1) ? 1f / (m_NumberOfSteps - 1) : 0.1f; }
		}
		Axis ScrollAxis
		{
			get { return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; }
		}
		bool ReverseValue
		{
			get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; }
		}
		public virtual void Rebuild( CanvasUpdate executing)
		{
		#if UNITY_EDITOR
			if( executing == CanvasUpdate.Prelayout)
			{
				m_OnValueChanged.Invoke( Value);
			}
		#endif
		}
		public virtual void LayoutComplete()
		{
		}
		public virtual void GraphicUpdateComplete()
		{
		}
		public virtual void SetValueWithoutNotify( float input)
		{
			Set(input, false);
		}
		void Set( float input, bool sendCallback = true)
		{
			float currentValue = m_Value;
			
			m_Value = input;
			
			if( currentValue == Value)
			{
				return;
			}
			UpdateVisuals();
			
			if( sendCallback != false)
			{
				UISystemProfilerApi.AddMarker( "Scrollbar.value", this);
				m_OnValueChanged.Invoke( Value);
			}
		}
		protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set( m_Value, false);
            UpdateVisuals();
        }
        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }
		protected internal virtual void Update()
        {
            if( m_DelayedUpdateVisuals != false)
            {
                m_DelayedUpdateVisuals = false;
                UpdateVisuals();
            }
        }
		void UpdateCachedReferences()
		{
			if( m_HandleRect && m_HandleRect.parent != null)
			{
				m_ContainerRect = m_HandleRect.parent.GetComponent<RectTransform>();
			}
			else
			{
				m_ContainerRect = null;
			}
		}
		void UpdateVisuals()
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				UpdateCachedReferences();
			}
		#endif
			m_Tracker.Clear();
			
			if( m_ContainerRect != null)
			{
				m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
				float movement = Mathf.Clamp01( Value) * (1 - Size);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;
				
				if( ReverseValue != false)
				{
					anchorMin[ (int)ScrollAxis] = 1 - movement - Size;
					anchorMax[ (int)ScrollAxis] = 1 - movement;
				}
				else
				{
					anchorMin[ (int)ScrollAxis] = movement;
					anchorMax[ (int)ScrollAxis] = movement + Size;
				}
				m_HandleRect.anchorMin = anchorMin;
				m_HandleRect.anchorMax = anchorMax;
			}
		}
		void UpdateDrag(PointerEventData eventData)
		{
			if( eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if( m_ContainerRect == null)
			{
				return;
			}
			Vector2 position = Vector2.zero;
			
			if( MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position) == false)
			{
				return;
			}
			UpdateDrag( m_ContainerRect, position, eventData.pressEventCamera);
		}
		void UpdateDrag( RectTransform containerRect, Vector2 position, Camera camera)
		{
			if( RectTransformUtility.ScreenPointToLocalPointInRectangle( containerRect, position, camera, out var localCursor) == false)
			{
				return;
			}
			Vector2 handleCenterRelativeToContainerCorner = localCursor - m_Offset - m_ContainerRect.rect.position;
			Vector2 handleCorner = handleCenterRelativeToContainerCorner - (m_HandleRect.rect.size - m_HandleRect.sizeDelta) * 0.5f;
			float parentSize = ScrollAxis == 0 ? m_ContainerRect.rect.width : m_ContainerRect.rect.height;
			float remainingSize = parentSize * (1 - Size);
			
			if( remainingSize <= 0)
			{
				return;
			}
			DoUpdateDrag( handleCorner, remainingSize);
		}
		private void DoUpdateDrag( Vector2 handleCorner, float remainingSize)
		{
			switch( m_Direction)
			{
				case Direction.LeftToRight:
				{
					Set( Mathf.Clamp01( handleCorner.x / remainingSize));
					break;
				}
				case Direction.RightToLeft:
				{
					Set( Mathf.Clamp01( 1.0f - (handleCorner.x / remainingSize)));
					break;
				}
				case Direction.BottomToTop:
				{
					Set( Mathf.Clamp01( handleCorner.y / remainingSize));
					break;
				}
				case Direction.TopToBottom:
				{
					Set( Mathf.Clamp01( 1.0f - (handleCorner.y / remainingSize)));
					break;
				}
			}
		}
		bool MayDrag( PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}
		public virtual void OnBeginDrag( PointerEventData eventData)
		{
			m_IsPointerDownAndNotDragging = false;
			
			if( MayDrag(eventData) == false)
			{
				return;
			}
			if( m_ContainerRect == null)
			{
				return;
			}
			m_Offset = Vector2.zero;
			
			if( RectTransformUtility.RectangleContainsScreenPoint( m_HandleRect, 
				eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera) != false)
			{
				if( RectTransformUtility.ScreenPointToLocalPointInRectangle(
					m_HandleRect, eventData.pointerPressRaycast.screenPosition, 
					eventData.pressEventCamera, out Vector2 localMousePos) != false)
				{
					m_Offset = localMousePos - m_HandleRect.rect.center;
				}
			}
		}
		public virtual void OnDrag( PointerEventData eventData)
		{
			if( MayDrag(eventData) == false)
			{
				return;
			}
			if( m_ContainerRect != null)
			{
				UpdateDrag( eventData);
			}
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			if( MayDrag( eventData) == false)
			{
				return;
			}
			base.OnPointerDown( eventData);
			m_IsPointerDownAndNotDragging = true;
			m_PointerDownRepeat = StartCoroutine( ClickRepeat( eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera));
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp( eventData);
			m_IsPointerDownAndNotDragging = false;
		}
		protected IEnumerator ClickRepeat( PointerEventData eventData)
		{
			return ClickRepeat( eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera);
		}
		protected IEnumerator ClickRepeat( Vector2 screenPosition, Camera camera)
		{
			while( m_IsPointerDownAndNotDragging)
			{
				if( RectTransformUtility.RectangleContainsScreenPoint( m_HandleRect, screenPosition, camera) == false)
				{
					UpdateDrag( m_ContainerRect, screenPosition, camera);
				}
				yield return new WaitForEndOfFrame();
			}
			StopCoroutine( m_PointerDownRepeat);
		}
		public override void OnMove(AxisEventData eventData)
		{
			if( IsActive() == false || IsInteractable() == false)
			{
				base.OnMove( eventData);
				return;
			}
			switch( eventData.moveDir)
			{
				case MoveDirection.Left:
				{
					if( ScrollAxis == Axis.Horizontal && FindSelectableOnLeft() == null)
					{
						Set( Mathf.Clamp01( ReverseValue ? Value + StepSize : Value - StepSize));
					}
					else
					{
						base.OnMove( eventData);
					}
					break;
				}
				case MoveDirection.Right:
				{
					if( ScrollAxis == Axis.Horizontal && FindSelectableOnRight() == null)
					{
						Set( Mathf.Clamp01(ReverseValue ? Value - StepSize : Value + StepSize));
					}
					else
					{
						base.OnMove( eventData);
					}
					break;
				}
				case MoveDirection.Up:
				{
					if( ScrollAxis == Axis.Vertical && FindSelectableOnUp() == null)
					{
						Set( Mathf.Clamp01( ReverseValue ? Value - StepSize : Value + StepSize));
					}
					else
					{
						base.OnMove( eventData);
					}
					break;
				}
				case MoveDirection.Down:
				{
					if( ScrollAxis == Axis.Vertical && FindSelectableOnDown() == null)
					{
						Set(Mathf.Clamp01( ReverseValue ? Value + StepSize : Value - StepSize));
					}
					else
					{
						base.OnMove( eventData);
					}
					break;
				}
			}
		}
		public override Selectable FindSelectableOnUp()
		{
			if( m_NavigateOnUp.Mode == NavigateMode.Automatic && ScrollAxis == Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnUp();
		}
		public override Selectable FindSelectableOnDown()
		{
			if( m_NavigateOnDown.Mode == NavigateMode.Automatic && ScrollAxis == Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnDown();
		}
		public override Selectable FindSelectableOnLeft()
		{
			if( m_NavigateOnLeft.Mode == NavigateMode.Automatic && ScrollAxis == Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnLeft();
		}
		public override Selectable FindSelectableOnRight()
		{
			if( m_NavigateOnRight.Mode == NavigateMode.Automatic && ScrollAxis == Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnRight();
		}
		public virtual void OnInitializePotentialDrag( PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}
		public void SetDirection( Direction direction, bool includeRectLayouts)
		{
			Axis oldAxis = ScrollAxis;
			bool oldReverse = ReverseValue;
			ScrollDirection = direction;
			
			if( includeRectLayouts == false)
			{
				return;
			}
			if( ScrollAxis != oldAxis)
			{
				RectTransformUtility.FlipLayoutAxes( transform as RectTransform, true, true);
			}
			if( ReverseValue != oldReverse)
			{
				RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)ScrollAxis, true, true);
			}
		}
		protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
			
            if( IsActive() == false)
			{
                return;
			}
            UpdateVisuals();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Size = Mathf.Clamp01(m_Size);
			
            if( IsActive() != false)
            {
                UpdateCachedReferences();
                Set( m_Value, false);
                m_DelayedUpdateVisuals = true;
            }
            if( UnityEditor.PrefabUtility.IsPartOfPrefabAsset( this) == false && Application.isPlaying == false)
			{
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild( this);
			}
        }

#endif
		[Serializable]
		public sealed class ScrollEvent : UnityEvent<float>{}
		[SerializeField]
		Direction m_Direction = Direction.LeftToRight;
		[SerializeField]
		RectTransform m_HandleRect;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_Value;
		[SerializeField, Range( 0, 11)]
		int m_NumberOfSteps = 0;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_Size = 0.2f;
		[SerializeField]
		ScrollEvent m_OnValueChanged = new();
		
		RectTransform m_ContainerRect;
		DrivenRectTransformTracker m_Tracker;
		Vector2 m_Offset = Vector2.zero;
		bool m_IsPointerDownAndNotDragging;
		Coroutine m_PointerDownRepeat;
		bool m_DelayedUpdateVisuals;
	}
}
