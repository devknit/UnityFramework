
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Knit.Framework
{
	[RequireComponent( typeof( RectTransform))]
	public class ScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
	{
		public enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped,
		}
		public enum ScrollbarVisibility
		{
			Permanent,
			AutoHide,
			AutoHideAndExpandViewport,
		}
		public RectTransform Content
		{
			get{ return m_Content; }
			set{ m_Content = value; }
		}
		public bool Horizontal
		{
			get{ return m_Horizontal; }
			set{ m_Horizontal = value; }
		}
		public bool Vertical
		{
			get{ return m_Vertical; }
			set{ m_Vertical = value; }
		}
		public MovementType MovementType_
		{
			get{ return m_MovementType; }
			set{ m_MovementType = value; }
		}
		public float Elasticity
		{
			get{ return m_Elasticity; }
			set{ m_Elasticity = value; }
		}
		public bool Inertia
		{
			get{ return m_Inertia; }
			set{ m_Inertia = value; }
		}
		public float DecelerationRate
		{
			get{ return m_DecelerationRate; }
			set{ m_DecelerationRate = value; }
		}
		public float ScrollSensitivity
		{
			get{ return m_ScrollSensitivity; }
			set{ m_ScrollSensitivity = value; }
		}
		public RectTransform Viewport
		{
			get{ return m_Viewport; }
			set{ m_Viewport = value; SetDirtyCaching(); }
		}
		public Scrollbar HorizontalScrollbar
		{
			get{ return m_HorizontalScrollbar; }
			set
			{
				if( m_HorizontalScrollbar != null)
				{
					m_HorizontalScrollbar.OnValueChanged.RemoveListener( SetHorizontalNormalizedPosition);
				}
				m_HorizontalScrollbar = value;
				
				if( m_Horizontal != false && m_HorizontalScrollbar != null)
				{
					m_HorizontalScrollbar.OnValueChanged.AddListener( SetHorizontalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}
		public Scrollbar VerticalScrollbar
		{
			get{ return m_VerticalScrollbar; }
			set
			{
				if( m_VerticalScrollbar != null)
				{
					m_VerticalScrollbar.OnValueChanged.RemoveListener( SetVerticalNormalizedPosition);
				}
				m_VerticalScrollbar = value;
				
				if( m_Vertical != false && m_VerticalScrollbar != null)
				{
					m_VerticalScrollbar.OnValueChanged.AddListener( SetVerticalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}
		public ScrollbarVisibility HorizontalScrollbarVisibility
		{
			get{ return m_HorizontalScrollbarVisibility; }
			set{ m_HorizontalScrollbarVisibility = value; SetDirtyCaching(); }
		}
		public ScrollbarVisibility VerticalScrollbarVisibility
		{
			get{ return m_VerticalScrollbarVisibility; }
			set{ m_VerticalScrollbarVisibility = value; SetDirtyCaching(); }
		}
		public float HorizontalScrollbarSpacing
		{
			get{ return m_HorizontalScrollbarSpacing; }
			set{ m_HorizontalScrollbarSpacing = value; SetDirty(); }
		}
		public float VerticalScrollbarSpacing
		{
			get{ return m_VerticalScrollbarSpacing; }
			set{ m_VerticalScrollbarSpacing = value; SetDirty(); }
		}
		public ScrollRectEvent OnValueChanged
		{
			get{ return m_OnValueChanged; }
			set{ m_OnValueChanged = value; }
		}
		protected RectTransform ViewRect
		{
			get
			{
				if( m_ViewRect == null)
				{
					m_ViewRect = m_Viewport;
				}
				if( m_ViewRect == null)
				{
					m_ViewRect = transform as RectTransform;
				}
				return m_ViewRect;
			}
		}
		public Vector2 Velocity
		{
			get{ return m_Velocity; }
			set{ m_Velocity = value; }
		}
		RectTransform RectTransform
		{
			get
			{
				if( m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}
		public Vector2 NormalizedPosition
		{
			get{ return new Vector2( HorizontalNormalizedPosition, VerticalNormalizedPosition); }
			set
			{
				SetNormalizedPosition( value.x, 0);
				SetNormalizedPosition( value.y, 1);
			}
		}
		public float HorizontalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				
				if( (m_ContentBounds.size.x <= m_ViewBounds.size.x) || Mathf.Approximately( m_ContentBounds.size.x, m_ViewBounds.size.x) != false)
				{
					return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
				}
				return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
			}
			set{ SetNormalizedPosition( value, 0); }
		}
		public float VerticalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				
				if ((m_ContentBounds.size.y <= m_ViewBounds.size.y) || Mathf.Approximately( m_ContentBounds.size.y, m_ViewBounds.size.y) != false)
				{
					return (m_ViewBounds.min.y > m_ContentBounds.min.y) ? 1 : 0;
				}
				return (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
			}
			set{ SetNormalizedPosition( value, 1); }
		}
		bool hScrollingNeeded
		{
			get
			{
				if( Application.isPlaying != false)
				{
					return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;
				}
				return true;
			}
		}
		bool vScrollingNeeded
		{
			get
			{
				if( Application.isPlaying != false)
				{
					return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
				}
				return true;
			}
		}
		public virtual float minWidth
		{
			get { return -1; }
		}
		public virtual float preferredWidth
		{
			get { return -1; }
		}
		public virtual float flexibleWidth
		{
			get { return -1; }
		}
		public virtual float minHeight
		{
			get { return -1; }
		}
		public virtual float preferredHeight
		{
			get { return -1; }
		}
		public virtual float flexibleHeight
		{
			get { return -1; }
		}
		public virtual int layoutPriority
		{
			get { return -1; }
		}
		public virtual void Rebuild( CanvasUpdate executing)
		{
			if( executing == CanvasUpdate.Prelayout)
			{
				UpdateCachedData();
			}
			if( executing == CanvasUpdate.PostLayout)
			{
				UpdateBounds();
				UpdateScrollbars( Vector2.zero);
				UpdatePrevData();
				m_HasRebuiltLayout = true;
			}
		}
		public virtual void LayoutComplete()
		{
		}
		public virtual void GraphicUpdateComplete()
		{
		}
		void UpdateCachedData()
		{
			Transform transform = this.transform;
			m_HorizontalScrollbarRect = m_HorizontalScrollbar == null ? null : m_HorizontalScrollbar.transform as RectTransform;
			m_VerticalScrollbarRect = m_VerticalScrollbar == null ? null : m_VerticalScrollbar.transform as RectTransform;
			
			bool viewIsChild = ViewRect.parent == transform;
			bool hScrollbarIsChild = !m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform;
			bool vScrollbarIsChild = !m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform;
			bool allAreChildren = viewIsChild && hScrollbarIsChild && vScrollbarIsChild;
			
			m_HSliderExpand = allAreChildren && m_HorizontalScrollbarRect && HorizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
			m_VSliderExpand = allAreChildren && m_VerticalScrollbarRect && VerticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
			m_HSliderHeight = m_HorizontalScrollbarRect == null ? 0 : m_HorizontalScrollbarRect.rect.height;
			m_VSliderWidth = m_VerticalScrollbarRect == null ? 0 : m_VerticalScrollbarRect.rect.width;
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			
			if( m_Horizontal != false && m_HorizontalScrollbar != null)
			{
				m_HorizontalScrollbar.OnValueChanged.AddListener( SetHorizontalNormalizedPosition);
			}
			if( m_Vertical != false && m_VerticalScrollbar != null)
			{
				m_VerticalScrollbar.OnValueChanged.AddListener( SetVerticalNormalizedPosition);
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild( this);
			SetDirty();
		}
		protected override void OnDisable()
		{
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild( this);
			
			if( m_HorizontalScrollbar != null)
			{
				m_HorizontalScrollbar.OnValueChanged.RemoveListener( SetHorizontalNormalizedPosition);
			}
			if( m_VerticalScrollbar != null)
			{
				m_VerticalScrollbar.OnValueChanged.RemoveListener( SetVerticalNormalizedPosition);
			}
			m_Dragging = false;
			m_Scrolling = false;
			m_HasRebuiltLayout = false;
			m_Tracker.Clear();
			m_Velocity = Vector2.zero;
			LayoutRebuilder.MarkLayoutForRebuild( RectTransform);
			base.OnDisable();
		}
		public override bool IsActive()
		{
			return base.IsActive() != false && m_Content != null;
		}
		void EnsureLayoutHasRebuilt()
		{
			if( m_HasRebuiltLayout == false && CanvasUpdateRegistry.IsRebuildingLayout() == false)
			{
				Canvas.ForceUpdateCanvases();
			}
		}
		public virtual void StopMovement()
		{
			m_Velocity = Vector2.zero;
		}
		public virtual void OnScroll( PointerEventData data)
		{
			if( IsActive() == false)
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			
			Vector2 delta = data.scrollDelta;
			delta.y *= -1;
			
			if( Vertical != false && Horizontal == false)
			{
				if( Mathf.Abs( delta.x) > Mathf.Abs( delta.y))
				{
					delta.y = delta.x;
				}
				delta.x = 0;
			}
			if( Horizontal != false && Vertical == false)
			{
				if( Mathf.Abs( delta.y) > Mathf.Abs( delta.x))
				{
					delta.x = delta.y;
				}
				delta.y = 0;
			}
			if( data.IsScrolling() != false)
			{
				m_Scrolling = true;
			}
			Vector2 position = m_Content.anchoredPosition;
			position += delta * m_ScrollSensitivity;
			
			if( m_MovementType == MovementType.Clamped)
			{
				position += CalculateOffset( position - m_Content.anchoredPosition);
			}
			SetContentAnchoredPosition( position);
			UpdateBounds();
		}
		public virtual void OnInitializePotentialDrag( PointerEventData eventData)
		{
			if( eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			m_Velocity = Vector2.zero;
		}
		public virtual void OnBeginDrag( PointerEventData eventData)
		{
			if( eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if( IsActive() == false)
			{
				return;
			}
			UpdateBounds();
			
			m_PointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( ViewRect, 
				eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
			m_ContentStartPosition = m_Content.anchoredPosition;
			m_Dragging = true;
		}
		public virtual void OnEndDrag( PointerEventData eventData)
		{
			if( eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			m_Dragging = false;
		}
		public virtual void OnDrag( PointerEventData eventData)
		{
			if( m_Dragging == false)
			{
				return;
			}
			if( eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if( IsActive() == false)
			{
				return;
			}
			if( RectTransformUtility.ScreenPointToLocalPointInRectangle( ViewRect, eventData.position, eventData.pressEventCamera, out Vector2 localCursor) == false)
			{
				return;
			}
			UpdateBounds();
			
			var pointerDelta = localCursor - m_PointerStartLocalCursor;
			Vector2 position = m_ContentStartPosition + pointerDelta;
			
			Vector2 offset = CalculateOffset( position - m_Content.anchoredPosition);
			position += offset;
			
			if( m_MovementType == MovementType.Elastic)
			{
				if( offset.x != 0)
				{
					position.x -= RubberDelta( offset.x, m_ViewBounds.size.x);
				}
				if( offset.y != 0)
				{
					position.y -= RubberDelta( offset.y, m_ViewBounds.size.y);
				}
			}
			SetContentAnchoredPosition( position);
		}
		protected virtual void SetContentAnchoredPosition( Vector2 position)
		{
			if( m_Horizontal == false)
			{
				position.x = m_Content.anchoredPosition.x;
			}
			if( m_Vertical == false)
			{
				position.y = m_Content.anchoredPosition.y;
			}
			if( position != m_Content.anchoredPosition)
			{
				m_Content.anchoredPosition = position;
				UpdateBounds();
			}
		}
		protected virtual void LateUpdate()
		{
			if( m_Content == false)
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			
			float deltaTime = Time.unscaledDeltaTime;
			Vector2 offset = CalculateOffset( Vector2.zero);
			
			if( deltaTime > 0.0f)
			{
				if( m_Dragging == false && (offset != Vector2.zero || m_Velocity != Vector2.zero))
				{
					Vector2 position = m_Content.anchoredPosition;
					
					for( int axis = 0; axis < 2; ++axis)
					{
						if( m_MovementType == MovementType.Elastic && offset[ axis] != 0)
						{
							float speed = m_Velocity[ axis];
							float smoothTime = m_Elasticity;
							
							if( m_Scrolling != false)
							{
								smoothTime *= 3.0f;
							}
							position[ axis] = Mathf.SmoothDamp( m_Content.anchoredPosition[ axis], 
								m_Content.anchoredPosition[ axis] + offset[ axis], ref speed, smoothTime, Mathf.Infinity, deltaTime);
							
							if( Mathf.Abs( speed) < 1)
							{
								speed = 0;
							}
							m_Velocity[ axis] = speed;
						}
						else if( m_Inertia != false)
						{
							m_Velocity[ axis] *= Mathf.Pow( m_DecelerationRate, deltaTime);
							
							if( Mathf.Abs( m_Velocity[ axis]) < 1)
							{
								m_Velocity[ axis] = 0;
							}
							position[ axis] += m_Velocity[ axis] * deltaTime;
						}
						else
						{
							m_Velocity[ axis] = 0;
						}
					}
					if( m_MovementType == MovementType.Clamped)
					{
						offset = CalculateOffset( position - m_Content.anchoredPosition);
						position += offset;
					}
					SetContentAnchoredPosition( position);
				}
				if( m_Dragging != false && m_Inertia != false)
				{
					Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
					m_Velocity = Vector3.Lerp( m_Velocity, newVelocity, deltaTime * 10);
				}
			}
			if( m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
			{
				UpdateScrollbars( offset);
				UISystemProfilerApi.AddMarker( "ScrollRect.value", this);
				m_OnValueChanged.Invoke( NormalizedPosition);
				UpdatePrevData();
			}
			UpdateScrollbarVisibility();
			m_Scrolling = false;
		}
		protected void UpdatePrevData()
		{
			if( m_Content == null)
			{
				m_PrevPosition = Vector2.zero;
			}
			else
			{
				m_PrevPosition = m_Content.anchoredPosition;
			}
			m_PrevViewBounds = m_ViewBounds;
			m_PrevContentBounds = m_ContentBounds;
		}
		private void UpdateScrollbars( Vector2 offset)
		{
			if( m_HorizontalScrollbar != null)
			{
				if( m_ContentBounds.size.x > 0)
				{
					m_HorizontalScrollbar.Size = Mathf.Clamp01( (m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x);
				}
				else
				{
					m_HorizontalScrollbar.Size = 1;
				}
				m_HorizontalScrollbar.Value = HorizontalNormalizedPosition;
			}
			if( m_VerticalScrollbar != null)
			{
				if( m_ContentBounds.size.y > 0)
				{
					m_VerticalScrollbar.Size = Mathf.Clamp01( (m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y);
				}
				else
				{
					m_VerticalScrollbar.Size = 1;
				}
				m_VerticalScrollbar.Value = VerticalNormalizedPosition;
			}
		}
		void SetHorizontalNormalizedPosition( float value)
		{
			SetNormalizedPosition( value, 0);
		}
		void SetVerticalNormalizedPosition( float value)
		{
			SetNormalizedPosition( value, 1);
		}
		protected virtual void SetNormalizedPosition( float value, int axis)
		{
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			
			float hiddenLength = m_ContentBounds.size[ axis] - m_ViewBounds.size[ axis];
			float contentBoundsMinPosition = m_ViewBounds.min[ axis] - value * hiddenLength;
			float newAnchoredPosition = m_Content.anchoredPosition[ axis] + contentBoundsMinPosition - m_ContentBounds.min[ axis];
			Vector3 anchoredPosition = m_Content.anchoredPosition;
			
			if( Mathf.Abs( anchoredPosition[ axis] - newAnchoredPosition) > 0.01f)
			{
				anchoredPosition[ axis] = newAnchoredPosition;
				m_Content.anchoredPosition = anchoredPosition;
				m_Velocity[ axis] = 0;
				UpdateBounds();
			}
		}
		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}
		public virtual void CalculateLayoutInputHorizontal()
		{
		}
		public virtual void CalculateLayoutInputVertical()
		{	
		}
		public virtual void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			UpdateCachedData();
			
			if( m_HSliderExpand != false || m_VSliderExpand != false)
			{
				m_Tracker.Add( this, ViewRect,
					DrivenTransformProperties.Anchors |
					DrivenTransformProperties.SizeDelta |
					DrivenTransformProperties.AnchoredPosition);
				
				ViewRect.anchorMin = Vector2.zero;
				ViewRect.anchorMax = Vector2.one;
				ViewRect.sizeDelta = Vector2.zero;
				ViewRect.anchoredPosition = Vector2.zero;
				
				LayoutRebuilder.ForceRebuildLayoutImmediate( Content);
				m_ViewBounds = new Bounds( ViewRect.rect.center, ViewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if( m_VSliderExpand != false && vScrollingNeeded != false)
			{
				ViewRect.sizeDelta = new Vector2( -(m_VSliderWidth + m_VerticalScrollbarSpacing), ViewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate( Content);
				m_ViewBounds = new Bounds( ViewRect.rect.center, ViewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if( m_HSliderExpand != false && hScrollingNeeded != false)
			{
				ViewRect.sizeDelta = new Vector2( ViewRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
				m_ViewBounds = new Bounds( ViewRect.rect.center, ViewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if( m_VSliderExpand != false && vScrollingNeeded != false && ViewRect.sizeDelta.x == 0 && ViewRect.sizeDelta.y < 0)
			{
				ViewRect.sizeDelta = new Vector2( -(m_VSliderWidth + m_VerticalScrollbarSpacing), ViewRect.sizeDelta.y);
			}
		}
		public virtual void SetLayoutVertical()
		{
			UpdateScrollbarLayout();
			m_ViewBounds = new Bounds( ViewRect.rect.center, ViewRect.rect.size);
			m_ContentBounds = GetBounds();
		}
		void UpdateScrollbarVisibility()
		{
			UpdateOneScrollbarVisibility( vScrollingNeeded, m_Vertical, m_VerticalScrollbarVisibility, m_VerticalScrollbar);
			UpdateOneScrollbarVisibility( hScrollingNeeded, m_Horizontal, m_HorizontalScrollbarVisibility, m_HorizontalScrollbar);
		}
		void UpdateScrollbarLayout()
		{
			if( m_VSliderExpand != false && m_HorizontalScrollbar != false)
			{
				m_Tracker.Add( this, m_HorizontalScrollbarRect,
					DrivenTransformProperties.AnchorMinX |
					DrivenTransformProperties.AnchorMaxX |
					DrivenTransformProperties.SizeDeltaX |
					DrivenTransformProperties.AnchoredPositionX);
				m_HorizontalScrollbarRect.anchorMin = new Vector2( 0, m_HorizontalScrollbarRect.anchorMin.y);
				m_HorizontalScrollbarRect.anchorMax = new Vector2( 1, m_HorizontalScrollbarRect.anchorMax.y);
				m_HorizontalScrollbarRect.anchoredPosition = new Vector2( 0, m_HorizontalScrollbarRect.anchoredPosition.y);
				
				if( vScrollingNeeded != false)
				{
					m_HorizontalScrollbarRect.sizeDelta = new Vector2( -(m_VSliderWidth + m_VerticalScrollbarSpacing), m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					m_HorizontalScrollbarRect.sizeDelta = new Vector2( 0, m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			if( m_HSliderExpand != false && m_VerticalScrollbar != null)
			{
				m_Tracker.Add( this, m_VerticalScrollbarRect,
					DrivenTransformProperties.AnchorMinY |
					DrivenTransformProperties.AnchorMaxY |
					DrivenTransformProperties.SizeDeltaY |
					DrivenTransformProperties.AnchoredPositionY);
				m_VerticalScrollbarRect.anchorMin = new Vector2( m_VerticalScrollbarRect.anchorMin.x, 0);
				m_VerticalScrollbarRect.anchorMax = new Vector2( m_VerticalScrollbarRect.anchorMax.x, 1);
				m_VerticalScrollbarRect.anchoredPosition = new Vector2( m_VerticalScrollbarRect.anchoredPosition.x, 0);
				
				if( hScrollingNeeded != false)
				{
					m_VerticalScrollbarRect.sizeDelta = new Vector2( m_VerticalScrollbarRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
				}
				else
				{
					m_VerticalScrollbarRect.sizeDelta = new Vector2( m_VerticalScrollbarRect.sizeDelta.x, 0);
				}
			}
		}
		protected void UpdateBounds()
		{
			m_ViewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
			m_ContentBounds = GetBounds();
			
			if (m_Content == null)
			{
				return;
			}
			Vector3 contentSize = m_ContentBounds.size;
			Vector3 contentPos = m_ContentBounds.center;
			var contentPivot = m_Content.pivot;
			AdjustBounds( ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
			m_ContentBounds.size = contentSize;
			m_ContentBounds.center = contentPos;
			
			if( m_MovementType == MovementType.Clamped)
			{
				Vector2 delta = Vector2.zero;
				
				if( m_ViewBounds.max.x > m_ContentBounds.max.x)
				{
					delta.x = System.Math.Min( m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
				}
				else if( m_ViewBounds.min.x < m_ContentBounds.min.x)
				{
					delta.x = System.Math.Max( m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
				}
				if( m_ViewBounds.min.y < m_ContentBounds.min.y)
				{
					delta.y = System.Math.Max( m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
				}
				else if( m_ViewBounds.max.y > m_ContentBounds.max.y)
				{
					delta.y = System.Math.Min( m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
				}
				if( delta.sqrMagnitude > float.Epsilon)
				{
					contentPos = m_Content.anchoredPosition + delta;
					if( m_Horizontal == false)
					{
						contentPos.x = m_Content.anchoredPosition.x;
					}
					if( m_Vertical == false)
					{
						contentPos.y = m_Content.anchoredPosition.y;
					}
					AdjustBounds( ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
				}
			}
		}
		internal static void AdjustBounds( ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
		{
			Vector3 excess = viewBounds.size - contentSize;
			
			if( excess.x > 0)
			{
				contentPos.x -= excess.x * (contentPivot.x - 0.5f);
				contentSize.x = viewBounds.size.x;
			}
			if( excess.y > 0)
			{
				contentPos.y -= excess.y * (contentPivot.y - 0.5f);
				contentSize.y = viewBounds.size.y;
			}
		}
		private Bounds GetBounds()
		{
			if( m_Content == null)
			{
				return new Bounds();
			}
			m_Content.GetWorldCorners( m_Corners);
			var viewWorldToLocalMatrix = ViewRect.worldToLocalMatrix;
			return InternalGetBounds( m_Corners, ref viewWorldToLocalMatrix);
		}
		internal static Bounds InternalGetBounds( Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
		{
			var vMin = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue);
			var vMax = new Vector3( float.MinValue, float.MinValue, float.MinValue);
			
			for( int i0 = 0; i0 < 4; ++i0)
			{
				Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4( corners[ i0]);
				vMin = Vector3.Min( v, vMin);
				vMax = Vector3.Max( v, vMax);
			}
			var bounds = new Bounds( vMin, Vector3.zero);
			bounds.Encapsulate( vMax);
			return bounds;
		}
		Vector2 CalculateOffset( Vector2 delta)
		{
			return InternalCalculateOffset( ref m_ViewBounds, ref m_ContentBounds, m_Horizontal, m_Vertical, m_MovementType, ref delta);
		}
		internal static Vector2 InternalCalculateOffset( ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal, bool vertical, MovementType movementType, ref Vector2 delta)
		{
			Vector2 offset = Vector2.zero;
			
			if( movementType == MovementType.Unrestricted)
			{
				return offset;
			}
			Vector2 min = contentBounds.min;
			Vector2 max = contentBounds.max;
			
			if( horizontal != false)
			{
				min.x += delta.x;
				max.x += delta.x;
				
				float maxOffset = viewBounds.max.x - max.x;
				float minOffset = viewBounds.min.x - min.x;
				
				if( minOffset < -0.001f)
				{
					offset.x = minOffset;
				}
				else if( maxOffset > 0.001f)
				{
					offset.x = maxOffset;
				}
			}
			if( vertical != false)
			{
				min.y += delta.y;
				max.y += delta.y;
				
				float maxOffset = viewBounds.max.y - max.y;
				float minOffset = viewBounds.min.y - min.y;
				
				if( maxOffset > 0.001f)
				{
					offset.y = maxOffset;
				}
				else if( minOffset < -0.001f)
				{
					offset.y = minOffset;
				}
			}
			return offset;
		}
		protected void SetDirty()
		{
			if( IsActive() == false)
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild( RectTransform);
		}
		protected void SetDirtyCaching()
		{
			if( IsActive() == false)
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild( this);
			LayoutRebuilder.MarkLayoutForRebuild( RectTransform);
			m_ViewRect = null;
		}
	#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetDirtyCaching();
		}
	#endif
		static float RubberDelta( float overStretching, float viewSize)
		{
			return (1 - (1 / ((Mathf.Abs( overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign( overStretching);
		}
		static void UpdateOneScrollbarVisibility( bool xScrollingNeeded, bool xAxisEnabled, ScrollbarVisibility scrollbarVisibility, Scrollbar scrollbar)
		{
			if( scrollbar != null)
			{
				if( scrollbarVisibility == ScrollbarVisibility.Permanent)
				{
					if( scrollbar.gameObject.activeSelf != xAxisEnabled)
					{
						scrollbar.gameObject.SetActive( xAxisEnabled);
					}
				}
				else
				{
					if( scrollbar.gameObject.activeSelf != xScrollingNeeded)
					{
						scrollbar.gameObject.SetActive( xScrollingNeeded);
					}
				}
			}
		}
		[System.Serializable]
		public sealed class ScrollRectEvent : UnityEvent<Vector2>{}
		
		[SerializeField]
		RectTransform m_Content;
		[SerializeField]
		bool m_Horizontal = true;
		[SerializeField]
		bool m_Vertical = true;
		[SerializeField]
		MovementType m_MovementType = MovementType.Elastic;
		[SerializeField]
		float m_Elasticity = 0.1f;
		[SerializeField]
		bool m_Inertia = true;
		[SerializeField]
		float m_DecelerationRate = 0.135f;
		[SerializeField]
		float m_ScrollSensitivity = 1.0f;
		[SerializeField]
		RectTransform m_Viewport;
		[SerializeField]
		Scrollbar m_HorizontalScrollbar;
		[SerializeField]
		Scrollbar m_VerticalScrollbar;
		[SerializeField]
		ScrollbarVisibility m_HorizontalScrollbarVisibility;
		[SerializeField]
		ScrollbarVisibility m_VerticalScrollbarVisibility;
		[SerializeField]
		float m_HorizontalScrollbarSpacing;
		[SerializeField]
		float m_VerticalScrollbarSpacing;
		[SerializeField]
		ScrollRectEvent m_OnValueChanged = new();
		
		Vector2 m_PointerStartLocalCursor;
		Vector2 m_ContentStartPosition;
		RectTransform m_ViewRect;
		Bounds m_ContentBounds;
		Bounds m_ViewBounds;
		Vector2 m_Velocity;
		bool m_Dragging;
		bool m_Scrolling;
		Vector2 m_PrevPosition;
		Bounds m_PrevContentBounds;
		Bounds m_PrevViewBounds;
		bool m_HasRebuiltLayout;
		bool m_HSliderExpand;
		bool m_VSliderExpand;
		float m_HSliderHeight;
		float m_VSliderWidth;
		RectTransform m_Rect;
		RectTransform m_HorizontalScrollbarRect;
		RectTransform m_VerticalScrollbarRect;
		DrivenRectTransformTracker m_Tracker;
		readonly Vector3[] m_Corners = new Vector3[ 4];
	}
}
