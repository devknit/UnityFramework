
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( Selectable), true), CanEditMultipleObjects]
	public class SelectableEditor : UnityEditor.Editor
	{
		Dictionary<string, SerializedProperty> m_Properties = new();
		
		protected virtual void OnEnable()
		{
			s_Editors.Add( this);
			RegisterStaticOnSceneGUI();
			s_ShowNavigateFold = EditorPrefs.GetBool( kShowNavigateFoldKey);
			s_ShowNavigateGizmo = EditorPrefs.GetBool( kShowNavigateGizmoKey);
			s_ShowNavigatePreset = (NavigateMode)EditorPrefs.GetInt( kShowNavigatePresetKey);
			
			SerializedProperty serializedProperty = serializedObject.GetIterator();
			
			serializedProperty.NextVisible( true);
			
			while( serializedProperty.NextVisible( false) != false)
			{
				m_Properties.Add( serializedProperty.name, serializedObject.FindProperty( serializedProperty.name));
			}
			m_ModuleHandleProperty = PopProperty( "m_ModuleHandle");
			m_ModuleSettingProperty = PopProperty( "m_ModuleSetting");
			m_InteractableProperty = PopProperty( "m_Interactable");
			m_IgnoreTimeScaleProperty = PopProperty( "m_IgnoreTimeScale");
			m_OrderProperty = PopProperty( "m_Order");
			m_GraphicProperty = PopProperty( "m_Graphic");
			m_OverrideSpritesProperty = PopProperty( "m_OverrideSprites");
			m_NavigationProperty = PopProperty( "m_Navigation");
			
			if( m_NavigationProperty != null)
			{
				m_NavigationModeProperty = m_NavigationProperty.FindPropertyRelative( "m_Mode");
				m_NavigationWrapAroundProperty = m_NavigationProperty.FindPropertyRelative( "m_WrapAround");
				m_NavigationSelectOnUpProperty = m_NavigationProperty.FindPropertyRelative( "m_SelectOnUp");
				m_NavigationSelectOnDownProperty = m_NavigationProperty.FindPropertyRelative( "m_SelectOnDown");
				m_NavigationSelectOnLeftProperty = m_NavigationProperty.FindPropertyRelative( "m_SelectOnLeft");
				m_NavigationSelectOnRightProperty = m_NavigationProperty.FindPropertyRelative( "m_SelectOnRight");
			}
			m_NavigateOnUpProperty = new NavigateProperties( PopProperty( "m_NavigateOnUp"));
			m_NavigateOnDownProperty = new NavigateProperties( PopProperty( "m_NavigateOnDown"));
			m_NavigateOnLeftProperty = new NavigateProperties( PopProperty( "m_NavigateOnLeft"));
			m_NavigateOnRightProperty = new NavigateProperties( PopProperty( "m_NavigateOnRight"));
			
			m_ActionTypeProperty = PopProperty( "m_ActionType");
			m_AudioTypeProperty = PopProperty( "m_AudioType");
			m_OnStateChangedProperty = PopProperty( "m_OnStateChanged");
		#if ENABLE_INPUT_SYSTEM
			m_ShortcutOnSubmitProperty = PopProperty( "m_ShortcutOnSubmit");
		#endif
		}
		protected virtual void OnDisable()
		{
			s_Editors.Remove( this);
			RegisterStaticOnSceneGUI();
		}
		protected SerializedProperty PopProperty( string name)
		{
			if( m_Properties.TryGetValue( name, out var property) != false)
			{
				m_Properties.Remove( name);
			}
			return property;
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			OnModuleGUI();
			OnStatusGUI();
			OnGraphicGUI();
			OnNavigationGUI();
			OnActionGUI();
			OnEventGUI();
			OnOtherGUI();
			serializedObject.ApplyModifiedProperties();
		}
		protected void OnModuleGUI()
		{
			if( m_ModuleHandleProperty.objectReferenceValue == null)
			{
				EditorGUILayout.PropertyField( m_ModuleHandleProperty);
			}
			if( m_ModuleSettingProperty.objectReferenceValue == null)
			{
				EditorGUILayout.PropertyField( m_ModuleSettingProperty);
			}
			if( m_ModuleHandleProperty.objectReferenceValue == null
			||	m_ModuleSettingProperty.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox( "Module not configured", MessageType.Error);
				
				if( GUILayout.Button( "Reset Module") != false)
				{
					foreach( var selectable in serializedObject.targetObjects.Cast<Selectable>())
					{
						selectable.ResetModule();
					}
				}
			}
		}
		protected void OnStatusGUI()
		{
			EditorGUILayout.PropertyField( m_InteractableProperty);
			EditorGUILayout.PropertyField( m_IgnoreTimeScaleProperty);
			EditorGUILayout.PropertyField( m_OrderProperty);
		}
		protected void OnGraphicGUI( GUIContent label=null)
		{
			if( label == null)
			{
				label = new GUIContent( m_GraphicProperty.displayName);
			}
			EditorGUILayout.PropertyField( m_GraphicProperty, label);
			// EditorGUILayout.PropertyField( m_OverrideSpritesProperty);
		}
		protected void OnEventGUI()
		{
			EditorGUILayout.PropertyField( m_OnStateChangedProperty);
		}
		protected void OnNavigationGUI()
		{
			if( m_NavigationProperty != null)
			{
				using( new EditorGUI.DisabledGroupScope( true))
				{
					Rect navigationRect = EditorGUILayout.GetControlRect();
					Rect navigationModeRect = navigationRect;
					Rect navigationToggleRect = navigationRect;
					navigationToggleRect.width = 32;
					navigationModeRect.xMax -= navigationToggleRect.width;
					navigationToggleRect.x = navigationModeRect.xMax;
					EditorGUI.PropertyField( navigationModeRect, m_NavigationModeProperty, new GUIContent( "Navigation"));
					EditorGUI.BeginChangeCheck();
					s_ShowNavigateGizmo = GUI.Toggle( navigationToggleRect, s_ShowNavigateGizmo, 
						EditorGUIUtility.TrIconContent( "d_gizmostoggle"), EditorStyles.miniButton);
					if( EditorGUI.EndChangeCheck() != false)
					{
						EditorPrefs.SetBool( kShowNavigateGizmoKey, s_ShowNavigateGizmo);
						SceneView.RepaintAll();
					}
					++EditorGUI.indentLevel;
					switch( m_NavigationModeProperty.intValue)
					{
						case (int)NavigationMode.Horizontal:
						case (int)NavigationMode.Vertical:
						{
							EditorGUILayout.PropertyField( m_NavigationWrapAroundProperty);
							break;
						}
						case (int)NavigationMode.Automatic:
						{
							break;
						}
						case (int)NavigationMode.Explicit:
						{
							EditorGUILayout.PropertyField( m_NavigationSelectOnUpProperty);
							EditorGUILayout.PropertyField( m_NavigationSelectOnDownProperty);
							EditorGUILayout.PropertyField( m_NavigationSelectOnLeftProperty);
							EditorGUILayout.PropertyField( m_NavigationSelectOnRightProperty);
							break;
						}
					}
					--EditorGUI.indentLevel;
				}
			}
			
			using( new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				
				if( GUILayout.Button( "Transfer Navigate") != false)
				{
					foreach( var target in targets.Cast<Selectable>())
					{
						target?.TransferNavigate();
					}
				}
			}
				
			{
				Rect navigationRect = EditorGUILayout.GetControlRect();
				Rect navigationModeRect = navigationRect;
				Rect navigationPresetRect = navigationRect;
				Rect navigationToggleRect = navigationRect;
				
				navigationToggleRect.width = 32;
				navigationPresetRect.xMin += EditorGUIUtility.labelWidth;
				navigationPresetRect.xMax -= navigationToggleRect.width;
				navigationModeRect.xMax -= navigationPresetRect.xMin;
				navigationToggleRect.x = navigationPresetRect.xMax;
				navigationPresetRect.xMin += 2;
				
				EditorGUI.BeginChangeCheck();
				s_ShowNavigateFold = EditorGUI.Foldout( navigationModeRect, s_ShowNavigateFold, new GUIContent( "Navigate"));
				if( EditorGUI.EndChangeCheck() != false)
				{
					EditorPrefs.SetBool( kShowNavigateFoldKey, s_ShowNavigateFold);
					SceneView.RepaintAll();
				}
				var dropDownToggleButtonStyle = typeof( EditorStyles).GetProperty( 
					"dropDownToggleButton", BindingFlags.Static | BindingFlags.NonPublic).GetValue( null) as GUIStyle;
				
				Rect rect = navigationPresetRect;
				Rect rectPopupButton = rect;
				rectPopupButton.x += rect.width - 16;
				rectPopupButton.width = 16;
				
				if( EditorGUI.DropdownButton( rectPopupButton, GUIContent.none, FocusType.Passive, GUIStyle.none) != false)
				{
					var menu = new GenericMenu();
					
					menu.AddItem( new GUIContent( "None"), false, () =>
					{
						s_ShowNavigatePreset = NavigateMode.None;
						
						foreach( var target in targets.Cast<Selectable>())
						{
							target.SetNavigateMode( s_ShowNavigatePreset);
							EditorUtility.SetDirty( target);
						}
						EditorPrefs.SetInt( kShowNavigatePresetKey, (int)s_ShowNavigatePreset);
						
					});
					menu.AddItem(new GUIContent( "Explicit"), false, () =>
					{
						s_ShowNavigatePreset = NavigateMode.Explicit;
						
						foreach( var target in targets.Cast<Selectable>())
						{
							target.SetNavigateMode( s_ShowNavigatePreset);
							EditorUtility.SetDirty( target);
						}
						EditorPrefs.SetInt( kShowNavigatePresetKey, (int)s_ShowNavigatePreset);
					});
					menu.AddItem(new GUIContent( "Automatic"), false, () =>
					{
						s_ShowNavigatePreset = NavigateMode.Automatic;
						
						foreach( var target in targets.Cast<Selectable>())
						{
							target.SetNavigateMode( s_ShowNavigatePreset);
							EditorUtility.SetDirty( target);
						}
						EditorPrefs.SetInt( kShowNavigatePresetKey, (int)s_ShowNavigatePreset);
					});
					menu.DropDown( rect);
				}
				else if( GUI.Button( rect, new GUIContent( s_ShowNavigatePreset.ToString()), dropDownToggleButtonStyle))
				{
					foreach( var target in targets.Cast<Selectable>())
					{
						target.SetNavigateMode( s_ShowNavigatePreset);
						EditorUtility.SetDirty( target);
					}
					GUIUtility.ExitGUI();
				}
				EditorGUI.BeginChangeCheck();
				s_ShowNavigateGizmo = GUI.Toggle( navigationToggleRect, s_ShowNavigateGizmo, 
					EditorGUIUtility.TrIconContent( "d_gizmostoggle"), EditorStyles.miniButton);
				if( EditorGUI.EndChangeCheck() != false)
				{
					EditorPrefs.SetBool( kShowNavigateGizmoKey, s_ShowNavigateGizmo);
					SceneView.RepaintAll();
				}
				if( s_ShowNavigateFold != false)
				{
					++EditorGUI.indentLevel;
					
					if( m_NavigateOnUpProperty.OnGUI() != false)
					{
						foreach( var target in targets.Cast<Selectable>())
						{
							target.FindAutomaticSelectableUp();
							EditorUtility.SetDirty( target);
						}
					}
					if( m_NavigateOnDownProperty.OnGUI() != false)
					{
						foreach( var target in targets.Cast<Selectable>())
						{
							target.FindAutomaticSelectableDown();
							EditorUtility.SetDirty( target);
						}
					}
					if( m_NavigateOnLeftProperty.OnGUI() != false)
					{
						foreach( var target in targets.Cast<Selectable>())
						{
							target.FindAutomaticSelectableLeft();
							EditorUtility.SetDirty( target);
						}
					}
					if( m_NavigateOnRightProperty.OnGUI() != false)
					{
						foreach( var target in targets.Cast<Selectable>())
						{
							target.FindAutomaticSelectableRight();
							EditorUtility.SetDirty( target);
						}
					}
					--EditorGUI.indentLevel;
				}
			}
		}
		protected void OnActionGUI()
		{
			if( m_ModuleSettingProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField( m_AudioTypeProperty);
				EditorGUILayout.PropertyField( m_ActionTypeProperty);
			}
			else
			{
				m_AudioTypeProperty.intValue = EditorGUILayout.IntField( m_AudioTypeProperty.displayName, m_AudioTypeProperty.intValue);
				m_ActionTypeProperty.intValue = EditorGUILayout.IntField( m_ActionTypeProperty.displayName, m_ActionTypeProperty.intValue);
			}
		}
		protected void OnOtherGUI()
		{
		#if ENABLE_INPUT_SYSTEM
			EditorGUILayout.PropertyField( m_ShortcutOnSubmitProperty);
		#endif
			EditorGUILayout.Space();
			
			foreach( var property in m_Properties.Values)
			{
				EditorGUILayout.PropertyField( property);
			}
		} 
		void RegisterStaticOnSceneGUI()
		{
			SceneView.duringSceneGui -= StaticOnSceneGUI;
			
			if( s_Editors.Count > 0)
			{
				SceneView.duringSceneGui += StaticOnSceneGUI;
			}
		}
		static void StaticOnSceneGUI( SceneView view)
		{
			if( s_ShowNavigateGizmo != false)
			{
				Selectable[] selectables = Selectable.AllSelectablesArray;
				
				for( int i0 = 0; i0 < selectables.Length; ++i0)
				{
					Selectable selectable = selectables[ i0];
					
					if( StageUtility.IsGameObjectRenderedByCamera( selectable.gameObject, Camera.current) != false)
					{
						DrawNavigationForSelectable( selectable);
					}
				}
			}
		}
		static void DrawNavigationForSelectable( Selectable selectable)
		{
			if( selectable != null)
			{
				Transform transform = selectable.transform;
				bool active = Selection.transforms.Any( x => x == transform);
				
				Handles.color = new Color( 1.0f, 0.6f, 0.2f, active ? 1.0f : 0.4f);
				DrawNavigationArrow( -Vector2.right, selectable, selectable.FindSelectableOnLeft());
				DrawNavigationArrow( Vector2.up, selectable, selectable.FindSelectableOnUp());
				
				Handles.color = new Color( 1.0f, 0.9f, 0.1f, active ? 1.0f : 0.4f);
				DrawNavigationArrow( Vector2.right, selectable, selectable.FindSelectableOnRight());
				DrawNavigationArrow( -Vector2.up, selectable, selectable.FindSelectableOnDown());
			}
		}
		static void DrawNavigationArrow( Vector2 direction, Selectable fromObj, Selectable toObj)
		{
			if( fromObj != null && toObj != null)
			{
				const float kArrowThickness = 2.5f;
				const float kArrowHeadSize = 1.2f;
				
				Transform fromTransform = fromObj.transform;
				Transform toTransform = toObj.transform;
				
				var sideDir = new Vector2( direction.y, -direction.x);
				Vector3 fromPoint = fromTransform.TransformPoint(
					GetPointOnRectEdge( fromTransform as RectTransform, direction));
				Vector3 toPoint = toTransform.TransformPoint(
					GetPointOnRectEdge( toTransform as RectTransform, -direction));
				float fromSize = HandleUtility.GetHandleSize( fromPoint) * 0.05f;
				float toSize = HandleUtility.GetHandleSize( toPoint) * 0.05f;
				fromPoint += fromTransform.TransformDirection( sideDir) * fromSize;
				toPoint += toTransform.TransformDirection( sideDir) * toSize;
				float length = Vector3.Distance( fromPoint, toPoint);
				Vector3 fromTangent = fromTransform.rotation * direction * length * 0.3f;
				Vector3 toTangent = toTransform.rotation * -direction * length * 0.3f;
				
				Handles.DrawBezier( fromPoint, toPoint, fromPoint + fromTangent, toPoint + toTangent, Handles.color, null, kArrowThickness);
				Handles.DrawAAPolyLine( kArrowThickness, toPoint, toPoint + toTransform.rotation * (-direction - sideDir) * toSize * kArrowHeadSize);
				Handles.DrawAAPolyLine( kArrowThickness, toPoint, toPoint + toTransform.rotation * (-direction + sideDir) * toSize * kArrowHeadSize);
			}
		}
		static Vector3 GetPointOnRectEdge( RectTransform rect, Vector2 direction)
		{
			if( rect == null)
			{
				return Vector3.zero;
			}
			if( direction != Vector2.zero)
			{
				direction /= Mathf.Max( Mathf.Abs( direction.x), Mathf.Abs( direction.y));
			}
			return rect.rect.center + Vector2.Scale( rect.rect.size, direction * 0.5f);
		}
		sealed class NavigateProperties
		{
			internal NavigateProperties( SerializedProperty rootProperty)
			{
				m_RootProperty = rootProperty;
				
				if( m_RootProperty != null)
				{
					m_ModeProperty = m_RootProperty.FindPropertyRelative( "m_Mode");
					m_TargetProperty = m_RootProperty.FindPropertyRelative( "m_Target");
					m_CorrectionProperty = m_RootProperty.FindPropertyRelative( "m_Correction");
					m_WrapAroundProperty = m_RootProperty.FindPropertyRelative( "m_WrapAround");
				}
			}
			internal bool OnGUI()
			{
				EditorGUILayout.PropertyField( m_ModeProperty, new GUIContent( m_RootProperty.displayName.Replace( "Navigate On ", string.Empty)));
				NavigateMode mode = (NavigateMode)m_ModeProperty.enumValueFlag;
				bool ret = false;
				
				if( mode != NavigateMode.None)
				{
					++EditorGUI.indentLevel;
					Color color = GUI.contentColor;
					GUI.contentColor = (mode != NavigateMode.Explicit)? Color.gray : color;
					
					Rect targetRect = EditorGUILayout.GetControlRect();
					Rect targetFieldRect = targetRect;
					Rect targetButtonRect = targetRect;
					targetButtonRect.width = 32;
					targetFieldRect.xMax -= targetButtonRect.width;
					targetButtonRect.x = targetFieldRect.xMax;
					EditorGUI.PropertyField( targetFieldRect, m_TargetProperty);
					
					if( GUI.Button( targetButtonRect, EditorGUIUtility.TrIconContent( "d_Search Icon"), EditorStyles.miniButton) != false)
					{
						ret = true;
					}
					GUI.contentColor = (mode != NavigateMode.Automatic)? Color.gray : color;
					EditorGUILayout.PropertyField( m_CorrectionProperty);
					EditorGUILayout.PropertyField( m_WrapAroundProperty);
					GUI.contentColor = color;
					--EditorGUI.indentLevel;
				}
				return ret;
			}
			SerializedProperty m_RootProperty;
			SerializedProperty m_ModeProperty;
			SerializedProperty m_TargetProperty;
			SerializedProperty m_CorrectionProperty;
			SerializedProperty m_WrapAroundProperty;
		}
		const string kShowNavigateFoldKey = "SelectableEditor.ShowNavigateFold";
		const string kShowNavigateGizmoKey = "SelectableEditor.ShowNavigateGizmo";
		const string kShowNavigatePresetKey = "SelectableEditor.ShowNavigatePreset";
		static readonly List<SelectableEditor> s_Editors = new();
		static bool s_ShowNavigateFold = false;
		static bool s_ShowNavigateGizmo = false;
		static NavigateMode s_ShowNavigatePreset = NavigateMode.Automatic;
		
		protected SerializedProperty m_GraphicProperty;
		protected SerializedProperty m_ModuleHandleProperty;
		protected SerializedProperty m_ModuleSettingProperty;
		protected SerializedProperty m_InteractableProperty;
		protected SerializedProperty m_IgnoreTimeScaleProperty;
		protected SerializedProperty m_OrderProperty;
		protected SerializedProperty m_NavigationProperty;
		protected SerializedProperty m_NavigationModeProperty;
		protected SerializedProperty m_NavigationWrapAroundProperty;
		protected SerializedProperty m_NavigationSelectOnUpProperty;
		protected SerializedProperty m_NavigationSelectOnDownProperty;
		protected SerializedProperty m_NavigationSelectOnLeftProperty;
		protected SerializedProperty m_NavigationSelectOnRightProperty;
		
		NavigateProperties m_NavigateOnUpProperty;
		NavigateProperties m_NavigateOnDownProperty;
		NavigateProperties m_NavigateOnLeftProperty;
		NavigateProperties m_NavigateOnRightProperty;
		
		protected SerializedProperty m_OverrideSpritesProperty;
		protected SerializedProperty m_ActionTypeProperty;
		protected SerializedProperty m_AudioTypeProperty;
		protected SerializedProperty m_OnStateChangedProperty;
	#if ENABLE_INPUT_SYSTEM
		protected SerializedProperty m_ShortcutOnSubmitProperty;
	#endif
	}
#if ENABLE_INPUT_SYSTEM
	[CustomPropertyDrawer( typeof( InputActionSet))]
	sealed class InputActionSetDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			if( property == null)
			{
				throw new System.ArgumentNullException( nameof( property));
			}
			var height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
			var useReference = property.FindPropertyRelative( "m_UseReference");
			return height + EditorGUI.GetPropertyHeight( property.FindPropertyRelative( 
				(useReference.boolValue == false)? "m_Action" : "m_Reference"));
		}
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			if( property == null)
			{
				throw new System.ArgumentNullException( nameof( property));
			}
			EditorGUI.BeginProperty( position, label, property);
			position.height = EditorGUIUtility.singleLineHeight;
			
			EditorGUI.LabelField( position, label);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			++EditorGUI.indentLevel;
			
			var ignoreInteractable = property.FindPropertyRelative( "m_IgnoreInteractable");
			EditorGUI.PropertyField( position, ignoreInteractable);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			var useReference = property.FindPropertyRelative( "m_UseReference");
			EditorGUI.PropertyField( position, useReference);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			if( useReference.boolValue == false)
			{
				var actionProperty = property.FindPropertyRelative( "m_Action");
				position.height = EditorGUI.GetPropertyHeight( actionProperty);
				EditorGUI.PropertyField( position, actionProperty);
			}
			else
			{
				var referenceProperty = property.FindPropertyRelative( "m_Reference");
				position.height = EditorGUI.GetPropertyHeight( referenceProperty);
				EditorGUI.PropertyField( position, referenceProperty);
			}
			--EditorGUI.indentLevel;
			EditorGUI.EndProperty();
		}
	}
#endif
}