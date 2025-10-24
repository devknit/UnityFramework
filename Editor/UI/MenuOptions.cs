
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Knit.Framework.Editor
{
	sealed class MenuOptions
	{
		enum MenuOptionsPriorityOrder
		{
			Button = 2005,
			Toggle = 2006,
			Slider = 2007,
			Scrollbar = 2008,
			ScrollRect = 2009,
		};
		[MenuItem( "GameObject/UI/Button - Framework", false, (int)MenuOptionsPriorityOrder.Button)]
		static public void AddButton( MenuCommand menuCommand)
		{
			var gameObject = new GameObject( "Button", typeof( RectTransform));
			PlaceUIElementRoot( gameObject, menuCommand);
			gameObject.AddComponent<Image>();
			gameObject.AddComponent<Button>();
		}
		[MenuItem( "GameObject/UI/Toggle - Framework", false, (int)MenuOptionsPriorityOrder.Toggle)]
		static public void AddToggle( MenuCommand menuCommand)
		{
			var toggleObject = new GameObject( "Toggle", typeof( RectTransform));
			PlaceUIElementRoot( toggleObject, menuCommand);
			toggleObject.AddComponent<Image>();
			toggleObject.AddComponent<Toggle>();
			
			var checkmarkObject = new GameObject( "Checkmark", typeof( RectTransform));
			SetParentAndAlign( checkmarkObject, toggleObject);
			checkmarkObject.AddComponent<Image>().sprite = GetStandardResources().checkmark;
			RectTransform checkmarkRect = checkmarkObject.GetComponent<RectTransform>();
			checkmarkRect.anchorMin = new Vector2( 0, 0);
			checkmarkRect.anchorMax = new Vector2( 1, 1);
			checkmarkRect.offsetMin = new Vector2( 0, 0);
			checkmarkRect.offsetMax = new Vector2( 0, 0);
		}
		[MenuItem( "GameObject/UI/Slider - Framework", false, (int)MenuOptionsPriorityOrder.Slider)]
		static public void AddSlider( MenuCommand menuCommand)
		{
			var sliderObject = new GameObject( "Slider");
			PlaceUIElementRoot( sliderObject, menuCommand);
			RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
			sliderRect.sizeDelta = new Vector2( 480, 60);
			Slider slider = sliderObject.AddComponent<Slider>();
			
			var backgroundObject = new GameObject( "Background");
			SetParentAndAlign( backgroundObject, sliderObject);
			RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
			backgroundRect.anchorMin = new Vector2( 0.0f, 0.25f);
			backgroundRect.anchorMax = new Vector2( 1.0f, 0.75f);
			backgroundRect.offsetMin = new Vector2( 0, 0);
			backgroundRect.offsetMax = new Vector2( 0, 0);
			Image backgraoundImage = backgroundObject.AddComponent<Image>();
			backgraoundImage.sprite = GetStandardResources().background;
			backgraoundImage.type = Image.Type.Sliced;
			
			var fillAreaObject = new GameObject( "FillArea");
			SetParentAndAlign( fillAreaObject, sliderObject);
			RectTransform fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
			fillAreaRect.anchorMin = new Vector2( 0.0f, 0.25f);
			fillAreaRect.anchorMax = new Vector2( 1.0f, 0.75f);
			fillAreaRect.offsetMin = new Vector2( 0, 0);
			fillAreaRect.offsetMax = new Vector2( 0, 0);
			
			var fillObject = new GameObject( "Fill");
			SetParentAndAlign( fillObject, fillAreaObject);
			RectTransform fillRect = fillObject.AddComponent<RectTransform>();
			fillRect.anchorMin = new Vector2( 0, 0);
			fillRect.anchorMax = new Vector2( 0, 1);
			fillRect.offsetMin = new Vector2( 0, 0);
			fillRect.offsetMax = new Vector2( 0, 0);
			Image fillImage = fillObject.AddComponent<Image>();
			fillImage.sprite = GetStandardResources().standard;
			fillImage.type = Image.Type.Sliced;
			
			var handleSlideAreaObject = new GameObject( "HandleSlideArea");
			SetParentAndAlign( handleSlideAreaObject, sliderObject);
			RectTransform handleSlideAreaRect = handleSlideAreaObject.AddComponent<RectTransform>();
			handleSlideAreaRect.anchorMin = new Vector2( 0, 0);
			handleSlideAreaRect.anchorMax = new Vector2( 1, 1);
			handleSlideAreaRect.offsetMin = new Vector2( 0, 0);
			handleSlideAreaRect.offsetMax = new Vector2( 0, 0);
			
			var handleObject = new GameObject( "Handle");
			SetParentAndAlign( handleObject, handleSlideAreaObject);
			RectTransform handleRect = handleObject.AddComponent<RectTransform>();
			handleRect.anchorMin = new Vector2( 0, 0);
			handleRect.anchorMax = new Vector2( 0, 1);
			handleRect.sizeDelta = new Vector2( 60, 0);
			Image handleImage = handleObject.AddComponent<Image>();
			handleImage.sprite = GetStandardResources().knob;
			
			slider.Graphic = handleImage;
			slider.m_FillRect = fillRect;
			slider.m_HandleRect = handleRect;
		}
		[MenuItem( "GameObject/UI/Scrollbar - Framework", false, (int)MenuOptionsPriorityOrder.Scrollbar)]
		static public void AddScrollbar( MenuCommand menuCommand)
		{
			CreateScrollbar( "Scrollbar", Scrollbar.Direction.LeftToRight, menuCommand, null);
		}
		[MenuItem( "GameObject/UI/ScrollRect - Framework", false, (int)MenuOptionsPriorityOrder.ScrollRect)]
		static public void AddScrollRect( MenuCommand menuCommand)
		{
			var scrollRectObject = new GameObject( "ScrollRect");
			PlaceUIElementRoot( scrollRectObject, menuCommand);
			RectTransform scrollRectRect = scrollRectObject.AddComponent<RectTransform>();
			scrollRectRect.sizeDelta = new Vector2( 200, 200);
			Image scrollRectImage = scrollRectObject.AddComponent<Image>();
			scrollRectImage.sprite = GetStandardResources().background;
			scrollRectImage.type = Image.Type.Sliced;
			ScrollRect scrollRect = scrollRectObject.AddComponent<ScrollRect>();
			
			var viewportObject = new GameObject( "Viewport");
			SetParentAndAlign( viewportObject, scrollRectObject);
			RectTransform viewportRect = viewportObject.AddComponent<RectTransform>();
			viewportRect.pivot = Vector2.up;
			viewportRect.anchorMin = Vector2.zero;
			viewportRect.anchorMax = Vector2.one;
			viewportRect.offsetMin = new Vector2( 0, 17);
			viewportRect.offsetMax = new Vector2( -17, 0);
			viewportObject.AddComponent<RectMask2D>();
			
			var contentObject = new GameObject( "Content");
			SetParentAndAlign( contentObject, viewportObject);
			RectTransform contentRect = contentObject.AddComponent<RectTransform>();
			contentRect.pivot = Vector2.up;
			contentRect.anchorMin = Vector2.up;
			contentRect.anchorMax = Vector2.one;
			contentRect.offsetMin = new Vector2( 0, -300);
			contentRect.offsetMax = new Vector2( 0, 0);
			
			scrollRect.Viewport = viewportRect;
			scrollRect.Content = contentRect;
			scrollRect.HorizontalScrollbar = CreateScrollbar( "ScrollbarHorizontal", 
				Scrollbar.Direction.LeftToRight, menuCommand, scrollRectObject);
			scrollRect.VerticalScrollbar = CreateScrollbar( "ScrollbarVertical", 
				Scrollbar.Direction.BottomToTop, menuCommand, scrollRectObject);
		}
		static Scrollbar CreateScrollbar( string objectName, 
			Scrollbar.Direction duration, MenuCommand menuCommand, GameObject parentObject)
		{
			var scrollbarObject = new GameObject( objectName);
			RectTransform sliderRect = scrollbarObject.AddComponent<RectTransform>();
			Image scrollbarImage = scrollbarObject.AddComponent<Image>();
			scrollbarImage.sprite = GetStandardResources().background;
			scrollbarImage.type = Image.Type.Sliced;
			Scrollbar scrollbar = scrollbarObject.AddComponent<Scrollbar>();
			scrollbar.SetDirection( duration, false);
			
			if( parentObject == null)
			{
				PlaceUIElementRoot( scrollbarObject, menuCommand);
				
				switch( duration)
				{
					case Scrollbar.Direction.LeftToRight:
					case Scrollbar.Direction.RightToLeft:
					{
						sliderRect.sizeDelta = new Vector2( 160, 20);
						break;
					}
					case Scrollbar.Direction.TopToBottom:
					case Scrollbar.Direction.BottomToTop:
					{
						sliderRect.sizeDelta = new Vector2( 20, 160);
						break;
					}
				}
			}
			else
			{
				SetParentAndAlign( scrollbarObject, parentObject);
				
				switch( duration)
				{
					case Scrollbar.Direction.LeftToRight:
					case Scrollbar.Direction.RightToLeft:
					{
						sliderRect.pivot = Vector2.zero;
						sliderRect.anchorMin = Vector2.zero;
						sliderRect.anchorMax = Vector2.right;
						sliderRect.offsetMin = new Vector2( 0, 0);
						sliderRect.offsetMax = new Vector2( -17, 20);
						break;
					}
					case Scrollbar.Direction.TopToBottom:
					case Scrollbar.Direction.BottomToTop:
					{
						sliderRect.pivot = Vector2.one;
						sliderRect.anchorMin = Vector2.right;
						sliderRect.anchorMax = Vector2.one;
						sliderRect.offsetMin = new Vector2( -20, 17);
						sliderRect.offsetMax = new Vector2( 0, 0);
						break;
					}
				}
				scrollbar.Value = 0.0f;
				scrollbar.Size = 1.0f;
			}
			var slidingAreaObject = new GameObject( "SlidingArea");
			SetParentAndAlign( slidingAreaObject, scrollbarObject);
			RectTransform backgroundRect = slidingAreaObject.AddComponent<RectTransform>();
			backgroundRect.anchorMin = Vector2.zero;
			backgroundRect.anchorMax = Vector2.one;
			backgroundRect.offsetMin = new Vector2( 10, 10);
			backgroundRect.offsetMax = new Vector2( -10, -10);
			
			var handleObject = new GameObject( "Handle");
			SetParentAndAlign( handleObject, slidingAreaObject);
			RectTransform handleRect = handleObject.AddComponent<RectTransform>();
			handleRect.anchorMin = new Vector2( 0, 0);
			handleRect.anchorMax = new Vector2( 0.2f, 1.0f);
			handleRect.offsetMin = new Vector2( -10, -10);
			handleRect.offsetMax = new Vector2( 10, 10);
			Image handleImage = handleObject.AddComponent<Image>();
			handleImage.sprite = GetStandardResources().standard;
			handleImage.type = Image.Type.Sliced;
			scrollbar.HandleRect = handleRect;
			
			return scrollbar;
		}
		static void PlaceUIElementRoot( GameObject element, MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			
			if( parent == null)
			{
				PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				parent = GetOrCreateCanvasGameObject();
				
				if (prefabStage != null && prefabStage.IsPartOfPrefabContents( parent) == false)
				{
					parent = prefabStage.prefabContentsRoot;
				}
			}
			if( parent.GetComponentsInParent<Canvas>( true).Length == 0)
			{
				GameObject canvas = CreateNewUI();
				Undo.SetTransformParent( canvas.transform, parent.transform, string.Empty);
				parent = canvas;
			}
			GameObjectUtility.EnsureUniqueNameForSibling( element);
			SetParentAndAlign( element, parent);
			
			Undo.RegisterFullObjectHierarchyUndo( (parent == null)? element : parent, string.Empty);
			Undo.SetCurrentGroupName( "Create " + element.name);
			Selection.activeGameObject = element;
		}
		static void SetParentAndAlign( GameObject child, GameObject parent)
		{
			if( parent != null)
			{
				Undo.SetTransformParent( child.transform, parent.transform, string.Empty);
				
				if( child.transform is RectTransform rectTransform)
				{
					rectTransform.anchoredPosition = Vector2.zero;
					Vector3 localPosition = rectTransform.localPosition;
					localPosition.z = 0;
					rectTransform.localPosition = localPosition;
				}
				else
				{
					child.transform.localPosition = Vector3.zero;
				}
				child.transform.localRotation = Quaternion.identity;
				child.transform.localScale = Vector3.one;
				SetLayerRecursively( child, parent.layer);
			}
		}
		static void SetLayerRecursively( GameObject gameObject, int layer)
		{
			Transform transform = gameObject.transform;
			gameObject.layer = layer;
			
			for( int i0 = 0; i0 < transform.childCount; ++i0)
			{
				SetLayerRecursively( transform.GetChild( i0).gameObject, layer);
			}
		}
		static public GameObject GetOrCreateCanvasGameObject()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			Canvas canvas = (activeGameObject == null)? null : 
				activeGameObject.GetComponentInParent<Canvas>();
			
			if( IsValidCanvas( canvas) != false)
			{
				return canvas.gameObject;
			}
			Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
			
			for( int i0 = 0; i0 < canvasArray.Length; ++i0)
			{
				if( IsValidCanvas( canvasArray[ i0]) != false)
				{
					return canvasArray[ i0].gameObject;
				}
			}
			return CreateNewUI();
		}
		static public GameObject CreateNewUI()
		{
			var root = ObjectFactory.CreateGameObject( "Canvas", 
				typeof( Canvas), typeof( CanvasScaler), typeof( GraphicRaycaster));
			root.layer = LayerMask.NameToLayer( kUILayerName);
			Canvas canvas = root.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			
			StageUtility.PlaceGameObjectInCurrentStage( root);
			PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			
			if( prefabStage != null)
			{
				Undo.SetTransformParent( root.transform, prefabStage.prefabContentsRoot.transform, string.Empty);
			}
			Undo.SetCurrentGroupName( "Create " + root.name);
			return root;
		}
		static bool IsValidCanvas( Canvas canvas)
		{
			if( canvas == null || canvas.gameObject.activeInHierarchy == false)
			{
				return false;
			}
			if( EditorUtility.IsPersistent( canvas) != false || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
			{
				return false;
			}
			return StageUtility.GetStageHandle( canvas.gameObject) == StageUtility.GetCurrentStageHandle();
		}
		static DefaultControls.Resources GetStandardResources()
		{
			if (s_StandardResources.standard == null)
			{
				s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>( kStandardSpritePath);
				s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>( kBackgroundSpritePath);
				s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>( kInputFieldBackgroundPath);
				s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>( kKnobPath);
				s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>( kCheckmarkPath);
				s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>( kDropdownArrowPath);
				s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>( kMaskPath);
			}
			return s_StandardResources;
		}
		const string kUILayerName = "UI";
		const string kStandardSpritePath = "UI/Skin/UISprite.psd";
		const string kBackgroundSpritePath = "UI/Skin/Background.psd";
		const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
		const string kKnobPath = "UI/Skin/Knob.psd";
		const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
		const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
		const string kMaskPath = "UI/Skin/UIMask.psd";
		static DefaultControls.Resources s_StandardResources;
	}
}