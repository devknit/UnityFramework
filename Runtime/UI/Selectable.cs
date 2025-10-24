
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Knit.Framework
{
	public enum SelectionState
	{
		Normal,
		Highlighted,
		Pressed,
		Selected,
		Disabled,
		Submited,
	}
	// [ExecuteAlways]
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract partial class Selectable: UIBehaviour
		, IPointerDownHandler, IPointerUpHandler
		, IPointerEnterHandler, IPointerExitHandler
		, ISelectHandler, IDeselectHandler, IMoveHandler
	{
		public Graphic Graphic
		{
			get { return m_Graphic; }
			set { m_Graphic = value; }
		}
		public ModuleHandle ModuleHandle
		{
			get{ return m_ModuleHandle; }
		}
		public int AudioType
		{
			get{ return m_AudioType; }
			set
			{
				if( m_AudioType != value)
				{
					m_AudioType = value;
					m_ModuleSetting?.TryGetSelectableAudios( m_AudioType, out m_SelectableAudio);
				}
			}
		}
		public int ActionType
		{
			get{ return m_ActionType; }
			set
			{
				if( m_ActionType != value)
				{
					m_ActionType = value;
					m_ModuleSetting?.TryGetSelectableActions( m_ActionType, out m_SelectableAction);
				}
			}
		}
		public int Order
		{
			get{ return m_Order; }
			set{ m_Order = value; }
		}
		public Selectable NavigateTargetUp
		{
			get{ return m_NavigateOnUp.Target; }
			set{ m_NavigateOnUp.Target = value; }
		}
		public Selectable NavigateTargetDown
		{
			get{ return m_NavigateOnDown.Target; }
			set{ m_NavigateOnDown.Target = value; }
		}
		public Selectable NavigateTargetLeft
		{
			get{ return m_NavigateOnLeft.Target; }
			set{ m_NavigateOnLeft.Target = value; }
		}
		public Selectable NavigateTargetRight
		{
			get{ return m_NavigateOnRight.Target; }
			set{ m_NavigateOnRight.Target = value; }
		}
		public event UnityAction<Selectable, SelectionState, bool> OnStateChanged
		{
			add{ m_OnStateChanged.AddListener( value); }
			remove{ m_OnStateChanged.RemoveListener( value); }
		}
	#if UNITY_EDITOR
		public event UnityAction<Selectable, SelectionState, bool> OnPersistentStateChanged
		{
			add{ UnityEditor.Events.UnityEventTools.AddPersistentListener( m_OnStateChanged, value); }
			remove{ UnityEditor.Events.UnityEventTools.RemovePersistentListener( m_OnStateChanged, value); }
		}
	#endif
		public bool Interactable
		{
			get { return m_Interactable; }
			set
			{
				if( m_Interactable != value)
				{
					if( value != false)
					{
						m_InternalState |= InternalState.InteractableSelf;
					}
					else
					{
						m_InternalState &= ~InternalState.InteractableSelf;
					}
					m_Interactable = value;
					OnSetProperty();
				}
			}
		}
		public bool IsInteractable()
		{
			return (m_InternalState & InternalState.InteractableMask) == InternalState.Interactable;
		}
		public bool HasNavigate()
		{
			return	m_NavigateOnUp.IsValid() != false
				||	m_NavigateOnDown.IsValid() != false
				||	m_NavigateOnLeft.IsValid() != false
				||	m_NavigateOnRight.IsValid() != false;
		}
		public bool HasVerticalNavigate()
		{
			return	m_NavigateOnUp.IsValid() != false
				||	m_NavigateOnDown.IsValid() != false;
		}
		public bool HasHorizontalNavigate()
		{
			return	m_NavigateOnLeft.IsValid() != false
				||	m_NavigateOnRight.IsValid() != false;
		}
		public virtual Selectable FindSelectableOnUp()
		{
			if( m_NavigateOnUp.Mode == NavigateMode.Explicit)
			{
				Selectable selectable = m_NavigateOnUp.Target;
				
				if( selectable != null)
				{
					if( selectable.isActiveAndEnabled == false || selectable.IsInteractable() == false)
					{
						selectable = selectable.FindSelectableOnUp();
					}
				}
				return selectable;
			}
			if( m_NavigateOnUp.Mode == NavigateMode.Automatic)
			{
				return FindSelectable( s_Selectables, s_SelectableCount, InternalState.Interactable, m_NavigateOnUp.Correction, m_NavigateOnUp.WrapAround, transform.rotation * Vector3.up);
			}
			return null;
		}
		public virtual Selectable FindSelectableOnDown()
		{
			if( m_NavigateOnDown.Mode == NavigateMode.Explicit)
			{
				Selectable selectable = m_NavigateOnDown.Target;
				
				if( selectable != null)
				{
					if( selectable.isActiveAndEnabled == false || selectable.IsInteractable() == false)
					{
						selectable = selectable.FindSelectableOnDown();
					}
				}
				return selectable;
			}
			if( m_NavigateOnDown.Mode == NavigateMode.Automatic)
			{
				return FindSelectable( s_Selectables, s_SelectableCount, InternalState.Interactable, m_NavigateOnDown.Correction, m_NavigateOnDown.WrapAround, transform.rotation * Vector3.down);
			}
			return null;
		}
		public virtual Selectable FindSelectableOnLeft()
		{
			if( m_NavigateOnLeft.Mode == NavigateMode.Explicit)
			{
				Selectable selectable = m_NavigateOnLeft.Target;
				
				if( selectable != null)
				{
					if( selectable.isActiveAndEnabled == false || selectable.IsInteractable() == false)
					{
						selectable = selectable.FindSelectableOnLeft();
					}
				}
				return selectable;
			}
			if( m_NavigateOnLeft.Mode == NavigateMode.Automatic)
			{
				return FindSelectable( s_Selectables, s_SelectableCount, InternalState.Interactable, m_NavigateOnLeft.Correction, m_NavigateOnLeft.WrapAround, transform.rotation * Vector3.left);
			}
			return null;
		}
		public virtual Selectable FindSelectableOnRight()
		{
			if( m_NavigateOnRight.Mode == NavigateMode.Explicit)
			{
				Selectable selectable = m_NavigateOnRight.Target;
				
				if( selectable != null)
				{
					if( selectable.isActiveAndEnabled == false || selectable.IsInteractable() == false)
					{
						selectable = selectable.FindSelectableOnRight();
					}
				}
				return selectable;
			}
			if( m_NavigateOnRight.Mode == NavigateMode.Automatic)
			{
				return FindSelectable( s_Selectables, s_SelectableCount, InternalState.Interactable, m_NavigateOnRight.Correction, m_NavigateOnRight.WrapAround, transform.rotation * Vector3.right);
			}
			return null;
		}
		public bool TryGetSelectionState( out SelectionState selectionState)
		{
			if( m_SelectionStateCache.HasValue != false)
			{
				selectionState = m_SelectionStateCache.Value;
				return true;
			}
			selectionState = SelectionState.Disabled;
			return false;
		}
	#if ENABLE_INPUT_SYSTEM
		public void SetShortcutOnSubmit( InputAction newAction)
		{
			if( (m_InternalState & InternalState.Enabled) != 0)
			{
				InputAction currentAction = m_ShortcutOnSubmit.Action;
				
				if( currentAction != null)
				{
					currentAction.started -= OnShortcut;
					// currentAction.Disable(); // 共用の場合があるのでひとまずは無効にはしない
				}
			}
			m_ShortcutOnSubmit = new InputActionSet( newAction, m_ShortcutOnSubmit.IgnoreInteractable);
			
			if( (m_InternalState & InternalState.Enabled) != 0 && newAction != null)
			{
				newAction.Enable();
				newAction.started += OnShortcut;
			}
		}
	#endif
		protected override void Awake()
		{
			if( m_Interactable != false)
			{
				m_InternalState |= InternalState.InteractableSelf;
			}
			else
			{
				m_InternalState &= ~InternalState.InteractableSelf;
			}
			if( m_Graphic == null)
			{
				m_Graphic = GetComponent<Graphic>();
			}
			if( m_ModuleHandle == null)
			{
				m_ModuleHandle = GetComponentInParent<ModuleHandle>();
			}
			if( m_ModuleSetting == null)
			{
				m_ModuleSetting = m_ModuleHandle?.m_ModuleSetting;
			}
			if( m_ModuleSetting != null)
			{
				m_ModuleSetting.TryGetSelectableActions( m_ActionType, out m_SelectableAction);
				m_ModuleSetting.TryGetSelectableAudios( m_AudioType, out m_SelectableAudio);
			}
			if( m_ModuleHandle != null)
			{
				m_ModuleHandle.OnFocus += OnModuleStateFocus;
			}
			m_InternalState |= InternalState.Awaked;
		}
		protected override void OnDestroy()
		{
			if( m_ModuleHandle != null)
			{
				m_ModuleHandle.OnFocus -= OnModuleStateFocus;
			}
		}
		protected override void OnEnable()
		{
			if( (m_InternalState & InternalState.Enabled) == 0)
			{
				base.OnEnable();
				
			#if ENABLE_INPUT_SYSTEM
				InputAction action = m_ShortcutOnSubmit.Action;
				
				if( action != null)
				{
					action.Enable();
					action.started += OnShortcut;
				}
			#endif
				if( s_SelectableCount == s_Selectables.Length)
				{
					Selectable[] selectables = new Selectable[ s_Selectables.Length * 2];
					Array.Copy(s_Selectables, selectables, s_Selectables.Length);
					s_Selectables = selectables;
				}
				if( EventSystem.current != null
				&&	EventSystem.current.currentSelectedGameObject == gameObject)
				{
					m_InternalState |= InternalState.EventSelect | InternalState.EventSelectUnconsumed;
				}
				m_CurrentIndex = s_SelectableCount;
				s_Selectables[ m_CurrentIndex] = this;
				++s_SelectableCount;
				
				m_InternalState &= ~InternalState.EventDown;
				m_InternalState |= InternalState.Enabled;
				DoStateTransition( GetCurrentSelectionState(), true);
			}
		}
		protected override void OnDisable()
		{
			if( (m_InternalState & InternalState.Enabled) != 0)
			{
			#if ENABLE_INPUT_SYSTEM
				InputAction action = m_ShortcutOnSubmit.Action;
				
				if( action != null)
				{
					action.started -= OnShortcut;
				}
			#endif
				s_SelectableCount--;
				s_Selectables[ s_SelectableCount].m_CurrentIndex = m_CurrentIndex;
				s_Selectables[ m_CurrentIndex] = s_Selectables[ s_SelectableCount];
				s_Selectables[ s_SelectableCount] = null;
				InstantClearState();
				DoStateTransition( GetCurrentSelectionState(), true);
				
				if( m_TweenCoroutine != null)
				{
					m_ModuleHandle.StopCoroutine( m_TweenCoroutine);
					m_TweenCoroutine = null;
				}
				base.OnDisable();
				m_InternalState &= ~InternalState.Enabled;
			}
		}
	#if ENABLE_INPUT_SYSTEM
		void OnShortcut( InputAction.CallbackContext context)
		{
			InternalState maskState = InternalState.InteractableMask;
			InternalState compState = InternalState.Interactable;
			
			if( m_ShortcutOnSubmit.IgnoreInteractable != false)
			{
				maskState &= ~(InternalState.InteractableSelf | InternalState.InteractableGroups);
				compState &= ~(InternalState.InteractableSelf | InternalState.InteractableGroups);
			}
			if( (m_InternalState & maskState) == compState)
			{
				OnSubmit( null);
			}
		}
	#endif
		void OnApplicationFocus( bool hasFocus)
		{
			if( hasFocus == false && IsInteractable() == false)
			{
				InstantClearState();
			}
		}
		void OnModuleStateFocus( bool enable)
		{
			if( (m_InternalState & InternalState.InteractableModule) != 0 != enable)
			{
				if( enable != false)
				{
					m_InternalState |= InternalState.InteractableModule;
				}
				else
				{
					m_InternalState &= ~(InternalState.InteractableModule | InternalState.EventSubmit);
				}
				OnSetProperty();
			}
		}
		protected override void OnCanvasGroupChanged()
		{
			if( (m_InternalState & InternalState.Awaked) == 0)
			{
				Awake();
			}
			var enable = ParentGroupAllowsInteraction( transform);
			
			if( (m_InternalState & InternalState.InteractableGroups) != 0 != enable)
			{
				if( enable != false)
				{
					m_InternalState |= InternalState.InteractableGroups;
				}
				else
				{
					m_InternalState &= ~(InternalState.InteractableGroups | InternalState.EventSubmit);
				}
				OnSetProperty();
			}
		}
		static bool ParentGroupAllowsInteraction( Transform rootTransform)
		{
			Transform transform = rootTransform;
			CanvasGroup canvasGroup;
			
			while( transform != null)
			{
				transform.GetComponents( s_CanvasGroupCache);
				
				for( int i0 = s_CanvasGroupCache.Count - 1; i0 >= 0; --i0)
				{
					canvasGroup = s_CanvasGroupCache[ i0];
					
					if( canvasGroup.enabled != false && canvasGroup.interactable == false)
					{
						return false;
					}
					if( canvasGroup.ignoreParentGroups)
					{
						return true;
					}
				}
				transform = transform.parent;
			}
			return true;
		}
		private protected void OnSetProperty()
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				DoStateTransition( GetCurrentSelectionState(), true);
			}
			else
		#endif
			{
				if( (m_InternalState & InternalState.Interactable) != InternalState.Interactable)
				{
					EventSystem ev = EventSystem.current;
					
					if( ev != null && ev.currentSelectedGameObject == gameObject)
					{
						ev.SetSelectedGameObject( null);
					}
				}
				DoStateTransition( GetCurrentSelectionState(), false);
			}
		}
		protected virtual void InstantClearState()
		{
			m_InternalState &= ~InternalState.Events;
		}
		protected SelectionState GetCurrentSelectionState( bool hasSubmit=true)
		{
			if( (m_InternalState & InternalState.EventSubmit) != 0 && hasSubmit != false)
			{
				return SelectionState.Submited;
			}
			if( isActiveAndEnabled == false || (m_InternalState & InternalState.Interactable) != InternalState.Interactable)
			{
				return SelectionState.Disabled;
			}
			if( (m_InternalState & InternalState.EventDown) != 0)
			{
				return SelectionState.Pressed;
			}
			if( (m_InternalState & InternalState.EventSelect) != 0)
			{
				return SelectionState.Selected;
			}
			if( (m_InternalState & InternalState.EventEnter) != 0)
			{
				return SelectionState.Highlighted;
			}
			return SelectionState.Normal;
		}
		void DoStateTransition( SelectionState state, bool instant)
		{
			if( m_SelectionStateCache != state)
			{
				SelectionState cacheState;
				
				if( m_SelectionStateCache.HasValue == false)
				{
					instant = true;
				}
			#if UNITY_EDITOR
				// Debug.Log( $"{name}: DoStateTransition( {m_SelectionStateCache}, {state}, {instant})\n{ToStateString()}", gameObject);
			#endif
				if( m_Graphic is Image image)
				{
					image.overrideSprite = m_OverrideSprites?.GetSprite( state);
				}
				if( m_SelectableAction == null)
				{
					if( instant == false)
					{
						OnActionCompleted( state);
					}
					cacheState = GetCurrentSelectionState( false);
				}
				else
				{
					if( instant != false)
					{
						if( m_TweenCoroutine != null)
						{
							m_ModuleHandle.StopCoroutine( m_TweenCoroutine);
							m_TweenCoroutine = null;
						}
						m_SelectableAction.SetValue( state, m_Graphic, transform);
					}
					else if( (m_ModuleHandle?.isActiveAndEnabled ?? false) != false)
					{
						if( m_TweenCoroutine != null)
						{
							m_ModuleHandle.StopCoroutine( m_TweenCoroutine);
							m_TweenCoroutine = null;
						}
						m_TweenCoroutine = m_ModuleHandle.StartCoroutine( 
							m_SelectableAction.OnTween( state, 
								m_Graphic, transform, m_IgnoreTimeScale, OnActionCompleted));
					}
					cacheState = state;
				}
				m_SelectionStateCache = cacheState;
				m_OnStateChanged.Invoke( this, cacheState, instant);
				
				bool unconsumedEventSelect = (m_InternalState & InternalState.EventSelectUnconsumed) != 0;
				
				if( state != SelectionState.Selected)
				{
					m_SelectableAudio?.Play( state);
				}
				else if( unconsumedEventSelect != false)
				{
					m_SelectableAudio?.Play( state);
					m_InternalState &= ~InternalState.EventSelectUnconsumed;
				}
			}
		}
		private protected virtual void OnActionCompleted( SelectionState state)
		{
			if( state == SelectionState.Submited)
			{
				m_InternalState &= ~InternalState.EventSubmit;
				OnSetProperty();
			}
		}
		public virtual void OnPointerDown( PointerEventData eventData)
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
					DoStateTransition( GetCurrentSelectionState(), false);
				}
			}
			eventData.eligibleForClick = isInteractable;
		}
		public virtual void OnPointerUp( PointerEventData eventData)
		{
			if( eventData.button == PointerEventData.InputButton.Left)
			{
				m_InternalState &= ~InternalState.EventDown;
				
				if( IsInteractable() != false)
				{
					DoStateTransition( GetCurrentSelectionState(), false);
				}
			}
		}
		public virtual void OnPointerEnter( PointerEventData eventData)
		{
			m_InternalState |= InternalState.EventEnter;
			
			if( IsInteractable() != false)
			{
				DoStateTransition( GetCurrentSelectionState(), false);
			}
		}
		public virtual void OnPointerExit( PointerEventData eventData)
		{
			m_InternalState &= ~InternalState.EventEnter;
			
			if( IsInteractable() != false)
			{
				DoStateTransition( GetCurrentSelectionState(), false);
			}
		}
		public virtual void OnSelect( BaseEventData eventData)
		{
			m_InternalState |= InternalState.EventSelect;
			
			if( (m_InternalState & InternalState.EventDown) == 0)
			{
				m_InternalState |= InternalState.EventSelectUnconsumed;
			}
			if( IsInteractable() != false)
			{
				DoStateTransition( GetCurrentSelectionState(), false);
			}
		}
		public virtual void OnDeselect( BaseEventData eventData)
		{
			m_InternalState &= ~InternalState.EventSelect | InternalState.EventSelectUnconsumed;
			
			if( IsInteractable() != false)
			{
				DoStateTransition( GetCurrentSelectionState(), false);
			}
		}
		public virtual void OnMove( AxisEventData eventData)
		{
			if( (m_ModuleHandle?.Interactable ?? false) != false)
			{
				switch( eventData.moveDir)
				{
					case MoveDirection.Right:
					{
						Navigate( eventData, FindSelectableOnRight());
						break;
					}
					case MoveDirection.Up:
					{
						Navigate( eventData, FindSelectableOnUp());
						break;
					}
					case MoveDirection.Left:
					{
						Navigate( eventData, FindSelectableOnLeft());
						break;
					}
					case MoveDirection.Down:
					{
						Navigate( eventData, FindSelectableOnDown());
						break;
					}
				}
			}
		}
		public virtual void OnSubmit( BaseEventData eventData)
		{
		}
		void Navigate( AxisEventData eventData, Selectable selectable)
		{
			if( selectable is Toggle targetToggle)
			{
				if( this is not Toggle currentToggle || currentToggle.m_ToggleGroup != targetToggle.m_ToggleGroup)
				{
					Toggle reselectable = targetToggle.m_ToggleGroup?.FindOnToggle();
					
					if( reselectable != null)
					{
						selectable = reselectable;
					}
				}
			}
			if( selectable != null && selectable.isActiveAndEnabled != false)
			{
				eventData.selectedObject = selectable.gameObject;
			}
		}
		Selectable FindSelectable( Selectable[] selectables, int selectableCount, 
			InternalState internalState, float correction, bool wantsWrapAround, Vector3 direction)
		{
			direction = direction.normalized;
			Vector3 localDir = Quaternion.Inverse( transform.rotation) * direction;
			Vector3 pos = transform.TransformPoint( GetPointOnRectEdge( transform as RectTransform, localDir));
			float maxScore = Mathf.NegativeInfinity;
			float maxFurthestScore = Mathf.NegativeInfinity;
			float score = 0;
			
			Selectable bestPick = null;
			Selectable bestFurthestPick = null;
			
			for( int i0 = 0; i0 < selectableCount; ++i0)
			{
				Selectable selectable = selectables[ i0];
				
				if( selectable == this
				||	(selectable.m_InternalState & internalState) != internalState
				||	selectable.HasNavigate() == false)
				{
					continue;
				}
			#if UNITY_EDITOR
				if( Camera.current != null
				&&	UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera( selectable.gameObject, Camera.current) == false)
				{
					continue;
				}
			#endif
				var rectTransform = selectable.transform as RectTransform;
				Vector3 selCenter = rectTransform != null ? (Vector3)rectTransform.rect.center : Vector3.zero;
				Vector3 vector = selectable.transform.TransformPoint( selCenter) - pos;
				float dot = Vector3.Dot( direction, vector.normalized);
				
				if( wantsWrapAround && dot < 0)
				{
					score = Mathf.Pow( -dot, correction) * vector.sqrMagnitude;
					
					if( score > maxFurthestScore)
					{
						maxFurthestScore = score;
						bestFurthestPick = selectable;
					}
					continue;
				}
				if( dot <= 0)
				{
					continue;
				}
				score = Mathf.Pow( dot, correction) / vector.sqrMagnitude;
				
				if( score > maxScore)
				{
					maxScore = score;
					bestPick = selectable;
				}
			}
			if( wantsWrapAround != false && bestPick == null)
			{
				return bestFurthestPick;
			}
			return bestPick;
		}
		static Vector3 GetPointOnRectEdge( RectTransform rectTransform, Vector2 direction)
		{
			if( rectTransform == null)
			{
				return Vector3.zero;
			}
			if( direction != Vector2.zero)
			{
				direction /= Mathf.Max( Mathf.Abs( direction.x), Mathf.Abs( direction.y));
			}
			direction = rectTransform.rect.center + Vector2.Scale( rectTransform.rect.size, direction * 0.5f);
			
			return direction;
		}
		protected Selectable()
		{
		}
	#if UNITY_EDITOR
		internal void FindAutomaticSelectableUp()
		{
			Transform parentTransform = FindRootObject( transform);
			
			if( parentTransform != null)
			{
				Selectable[] selectables = parentTransform.GetComponentsInChildren<Selectable>( false);
				
				m_NavigateOnUp.Target = FindSelectable( 
					selectables, selectables.Length, InternalState.InteractableSelf, 
					m_NavigateOnUp.Correction, m_NavigateOnUp.WrapAround, transform.rotation * Vector3.up);
			}
		}
		internal void FindAutomaticSelectableDown()
		{
			Transform parentTransform = FindRootObject( transform);
			
			if( parentTransform != null)
			{
				Selectable[] selectables = parentTransform.GetComponentsInChildren<Selectable>( false);
				
				m_NavigateOnDown.Target = FindSelectable( 
					selectables, selectables.Length, InternalState.InteractableSelf, 
					m_NavigateOnDown.Correction, m_NavigateOnDown.WrapAround, transform.rotation * Vector3.down);
			}
		}
		internal void FindAutomaticSelectableLeft()
		{
			Transform parentTransform = FindRootObject( transform);
			
			if( parentTransform != null)
			{
				Selectable[] selectables = parentTransform.GetComponentsInChildren<Selectable>( false);
				
				m_NavigateOnLeft.Target = FindSelectable( 
					selectables, selectables.Length, InternalState.InteractableSelf, 
					m_NavigateOnLeft.Correction, m_NavigateOnLeft.WrapAround, transform.rotation * Vector3.left);
			}
		}
		internal void FindAutomaticSelectableRight()
		{
			Transform parentTransform = FindRootObject( transform);
			
			if( parentTransform != null)
			{
				Selectable[] selectables = parentTransform.GetComponentsInChildren<Selectable>( false);
				
				m_NavigateOnRight.Target = FindSelectable( 
					selectables, selectables.Length, InternalState.InteractableSelf, 
					m_NavigateOnRight.Correction, m_NavigateOnRight.WrapAround, transform.rotation * Vector3.right);
			}
		}
		static Transform FindRootObject( Transform transform)
		{
			ModuleHandle moduleHandle = transform.GetComponentInParent<ModuleHandle>();
			
			if( moduleHandle != null)
			{
				return moduleHandle.transform;
			}
			
			while( transform.parent != null)
			{
				transform = transform.parent;
			}
			return transform;
		}
		internal void SetNavigateMode( NavigateMode mode)
		{
			m_NavigateOnUp.Mode = mode;
			m_NavigateOnDown.Mode = mode;
			m_NavigateOnLeft.Mode = mode;
			m_NavigateOnRight.Mode = mode;
		}
		internal void TransferNavigate()
		{
			NavigateMode vNode = m_Navigation.Mode switch
			{
				NavigationMode.Explicit => NavigateMode.Explicit,
				NavigationMode.Vertical => NavigateMode.Automatic,
				NavigationMode.Automatic => NavigateMode.Automatic,
				_ => NavigateMode.None
			};
			NavigateMode hNode = m_Navigation.Mode switch
			{
				NavigationMode.Explicit => NavigateMode.Explicit,
				NavigationMode.Horizontal => NavigateMode.Automatic,
				NavigationMode.Automatic => NavigateMode.Automatic,
				_ => NavigateMode.None
			};
			m_NavigateOnUp.Mode = vNode;
			m_NavigateOnDown.Mode = vNode;
			m_NavigateOnLeft.Mode = hNode;
			m_NavigateOnRight.Mode = hNode;
			m_NavigateOnUp.Target = m_Navigation.SelectOnUp;
			m_NavigateOnDown.Target = m_Navigation.SelectOnDown;
			m_NavigateOnLeft.Target = m_Navigation.SelectOnLeft;
			m_NavigateOnRight.Target = m_Navigation.SelectOnRight;
			UnityEditor.EditorUtility.SetDirty( this);
		}
		internal void ResetModule()
		{
			m_ModuleHandle = GetComponentInParent<ModuleHandle>();
			m_ModuleSetting = m_ModuleHandle?.m_ModuleSetting;
		}
		protected override void Reset()
		{
			m_Graphic = GetComponent<Graphic>();
			ResetModule();
		}
		protected override void OnValidate()
		{
			base.OnValidate();
			
			if( isActiveAndEnabled != false)
			{
				if( IsInteractable() == false
				&&	EventSystem.current != null
				&&	EventSystem.current.currentSelectedGameObject == gameObject)
				{
					EventSystem.current.SetSelectedGameObject( null);
				}
				DoStateTransition( GetCurrentSelectionState(), true);
			}
		}
		private protected string ToStateString()
		{
			return 
				$"State = {m_SelectionStateCache}\n"
				+$"Enabled = {(m_InternalState & InternalState.Enabled) != 0}\n"
				+$"InteractableSelf = {(m_InternalState & InternalState.InteractableSelf) != 0}\n"
				+$"InteractableModule = {(m_InternalState & InternalState.InteractableModule) != 0}\n"
				+$"InteractableGroups = {(m_InternalState & InternalState.InteractableGroups) != 0}\n"
				+$"EventEnter = {(m_InternalState & InternalState.EventEnter) != 0}\n"
				+$"EventSelect = {(m_InternalState & InternalState.EventSelect) != 0}\n"
				+$"EventSelectUnconsumed = {(m_InternalState & InternalState.EventSelectUnconsumed) != 0}\n"
				+$"EventDown = {(m_InternalState & InternalState.EventDown) != 0}\n"
				+$"EventSubmit = {(m_InternalState & InternalState.EventSubmit) != 0}\n";
		}
	#endif
		private protected enum InternalState
		{
			None = 0,
			Awaked = 1 << 0,				// 0000 0000 0000 0001, 0x0001
			Enabled = 1 << 1,				// 0000 0000 0000 0010, 0x0002
			InteractableSelf = 1 << 4,		// 0000 0000 0001 0000, 0x0010
			InteractableModule = 1 << 5,	// 0000 0000 0010 0000, 0x0020
			InteractableGroups = 1 << 6,	// 0000 0000 0100 0000, 0x0040
			EventEnter = 1 << 8,			// 0000 0001 0000 0000, 0x0100
			EventSelect = 1 << 9,			// 0000 0010 0000 0000, 0x0200
			EventDown = 1 << 10,			// 0000 0100 0000 0000, 0x0400
			EventSubmit = 1 << 11,			// 0000 1000 0000 0000, 0x0800
			EventSelectUnconsumed = 1 << 12,// 0001 0000 0000 0000, 0x1000
			Events = EventEnter | EventSelect | EventDown | EventSubmit | EventSelectUnconsumed,
			Default = InteractableSelf | InteractableGroups,
			Interactable = Enabled | InteractableSelf | InteractableModule | InteractableGroups,
			InteractableMask = Interactable | EventSubmit
		}
		[Serializable]
		sealed class OverrideSprites
		{
			internal Sprite GetSprite( SelectionState state)
			{
				return state switch
				{
					SelectionState.Normal => m_Normal,
					SelectionState.Highlighted => m_Highlighte,
					SelectionState.Pressed => m_Presse,
					SelectionState.Selected => m_Select,
					SelectionState.Disabled => m_Disable,
					SelectionState.Submited => m_Submit,
					_ => null
				};
			}
			[SerializeField]
			Sprite m_Disable;
			[SerializeField]
			Sprite m_Normal;
			[SerializeField]
			Sprite m_Highlighte;
			[SerializeField]
			Sprite m_Select;
			[SerializeField]
			Sprite m_Presse;
			[SerializeField]
			Sprite m_Submit;
		}
		[Serializable]
		sealed class StateChangedEvent : UnityEvent<Selectable, SelectionState, bool>
		{
		}
		[SerializeField]
		Graphic m_Graphic;
		[SerializeField]
		ModuleHandle m_ModuleHandle;
		[SerializeField]
		internal ModuleSetting m_ModuleSetting;
		[SerializeField]
		bool m_Interactable = true;
		[SerializeField]
		private protected bool m_IgnoreTimeScale;
		[SerializeField]
		int m_Order;
	#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		InputActionSet m_ShortcutOnSubmit;
	#endif
		[SerializeField]
		Navigation m_Navigation = Navigation.DefaultNavigation;
		[SerializeField]
		private protected Navigate m_NavigateOnUp = Framework.Navigate.DefaultNavigation;
		[SerializeField]
		private protected Navigate m_NavigateOnDown = Framework.Navigate.DefaultNavigation;
		[SerializeField]
		private protected Navigate m_NavigateOnLeft = Framework.Navigate.DefaultNavigation;
		[SerializeField]
		private protected Navigate m_NavigateOnRight = Framework.Navigate.DefaultNavigation;
		[SerializeField]
		OverrideSprites m_OverrideSprites;
		[SerializeField]
		StateChangedEvent m_OnStateChanged = new();
		[SerializeField, SelectableActions]
		int m_ActionType;
		[SerializeField, SelectableAudios]
		int m_AudioType;
		
		private protected InternalState m_InternalState = InternalState.Default;
		private protected SelectableAction m_SelectableAction;
		private protected SelectableAudio m_SelectableAudio;
		SelectionState? m_SelectionStateCache;
		Coroutine m_TweenCoroutine;
		
		internal static bool TryGetSelectable( Selectable selectable)
		{
			selectable = null;
			
			if( s_Selectables.Length > 0)
			{
				for( int i0 = 0; i0 < s_Selectables.Length && selectable == null; ++i0)
				{
					selectable = s_Selectables[ i0];
				}
			}
			return selectable != null;
		}
		public static IEnumerable<Selectable> OrderSelectables( int minOrder=int.MinValue)
		{
			return s_Selectables.Where( (x, i0) => 
				i0 < s_SelectableCount &&
				minOrder <= x.m_Order &&
				x.isActiveAndEnabled != false &&
				x.HasNavigate() != false &&
				x.IsInteractable() != false)
				.OrderBy( x => x.m_Order);
		}
		public static Selectable OrderSelectable( int minOrder=int.MinValue)
		{
			return OrderSelectables( minOrder).FirstOrDefault();
		}
		internal static Selectable[] AllSelectablesArray
		{
			get
			{
				Selectable[] buffer = new Selectable[ s_SelectableCount];
				Array.Copy( s_Selectables, buffer, s_SelectableCount);
				return buffer;
			}
		}
		private protected int m_CurrentIndex = -1;
		private protected static int s_SelectableCount = 0;
		private protected static Selectable[] s_Selectables = new Selectable[ 10];
		static readonly List<CanvasGroup> s_CanvasGroupCache = new();
	}
}
