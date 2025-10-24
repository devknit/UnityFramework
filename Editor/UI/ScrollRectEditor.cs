
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( ScrollRect), true), CanEditMultipleObjects]
	public class ScrollRectEditor : UnityEditor.Editor
	{
		void OnEnable()
		{
			m_ContentProperty = serializedObject.FindProperty( "m_Content");
			m_HorizontalProperty = serializedObject.FindProperty( "m_Horizontal");
			m_VerticalProperty = serializedObject.FindProperty( "m_Vertical");
			m_MovementTypeProperty = serializedObject.FindProperty( "m_MovementType");
			m_ElasticityProperty = serializedObject.FindProperty( "m_Elasticity");
			m_InertiaProperty = serializedObject.FindProperty( "m_Inertia");
			m_DecelerationRateProperty = serializedObject.FindProperty( "m_DecelerationRate");
			m_ScrollSensitivityProperty = serializedObject.FindProperty( "m_ScrollSensitivity");
			m_ViewportProperty = serializedObject.FindProperty( "m_Viewport");
			m_HorizontalScrollbarProperty = serializedObject.FindProperty( "m_HorizontalScrollbar");
			m_VerticalScrollbarProperty = serializedObject.FindProperty( "m_VerticalScrollbar");
			m_HorizontalScrollbarVisibilityProperty = serializedObject.FindProperty( "m_HorizontalScrollbarVisibility");
			m_VerticalScrollbarVisibilityProperty = serializedObject.FindProperty( "m_VerticalScrollbarVisibility");
			m_HorizontalScrollbarSpacingProperty = serializedObject.FindProperty( "m_HorizontalScrollbarSpacing");
			m_VerticalScrollbarSpacingProperty = serializedObject.FindProperty( "m_VerticalScrollbarSpacing");
			m_OnValueChangedProperty = serializedObject.FindProperty( "m_OnValueChanged");
			
			m_ShowElasticity = new AnimBool( Repaint);
            m_ShowDecelerationRate = new AnimBool( Repaint);
            SetAnimBools( true);
		}
		void OnDisable()
		{
			m_ShowElasticity.valueChanged.RemoveListener( Repaint);
			m_ShowDecelerationRate.valueChanged.RemoveListener( Repaint);
		}
		void SetAnimBools( bool instant)
		{
			SetAnimBool( m_ShowElasticity, m_MovementTypeProperty.hasMultipleDifferentValues == false && m_MovementTypeProperty.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
			SetAnimBool( m_ShowDecelerationRate, m_InertiaProperty.hasMultipleDifferentValues == false && m_InertiaProperty.boolValue == true, instant);
		}
		void SetAnimBool( AnimBool a, bool value, bool instant)
		{
			if( instant != false)
			{
				a.value = value;
			}
			else
			{
				a.target = value;
			}
		}
		void CalculateCachedValues()
		{
			m_ViewportIsNotChild = false;
			m_HScrollbarIsNotChild = false;
			m_VScrollbarIsNotChild = false;
			
			if( targets.Length == 1)
			{
				Transform transform = ((ScrollRect)target).transform;
				
				if( m_ViewportProperty.objectReferenceValue == null || ((RectTransform)m_ViewportProperty.objectReferenceValue).transform.parent != transform)
				{
					m_ViewportIsNotChild = true;
				}
				if( m_HorizontalScrollbarProperty.objectReferenceValue == null || ((Scrollbar)m_HorizontalScrollbarProperty.objectReferenceValue).transform.parent != transform)
				{
					m_HScrollbarIsNotChild = true;
				}
				if( m_VerticalScrollbarProperty.objectReferenceValue == null || ((Scrollbar)m_VerticalScrollbarProperty.objectReferenceValue).transform.parent != transform)
				{
					m_VScrollbarIsNotChild = true;
				}
			}
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			
			SetAnimBools( false);
			CalculateCachedValues();
			
			EditorGUILayout.PropertyField( m_ContentProperty);
            EditorGUILayout.PropertyField( m_HorizontalProperty);
            EditorGUILayout.PropertyField( m_VerticalProperty);
            EditorGUILayout.PropertyField( m_MovementTypeProperty);
			
			if( EditorGUILayout.BeginFadeGroup( m_ShowElasticity.faded) != false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField( m_ElasticityProperty);
                EditorGUI.indentLevel--;
            }
			EditorGUILayout.EndFadeGroup();
			
			EditorGUILayout.PropertyField( m_InertiaProperty);
			
			if( EditorGUILayout.BeginFadeGroup( m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField( m_DecelerationRateProperty);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
			
			EditorGUILayout.PropertyField( m_ScrollSensitivityProperty);
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField( m_ViewportProperty);
            EditorGUILayout.PropertyField( m_HorizontalScrollbarProperty);
			
            if( m_HorizontalScrollbarProperty.objectReferenceValue != null
			&&	m_HorizontalScrollbarProperty.hasMultipleDifferentValues == false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField( m_HorizontalScrollbarVisibilityProperty, EditorGUIUtility.TrTextContent( "Visibility"));
				
                if( (ScrollRect.ScrollbarVisibility)m_HorizontalScrollbarVisibilityProperty.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                &&	m_HorizontalScrollbarVisibilityProperty.hasMultipleDifferentValues == false)
                {
                    if( m_ViewportIsNotChild != false || m_HScrollbarIsNotChild != false)
					{
                        EditorGUILayout.HelpBox( s_HError, MessageType.Error);
					}
                    EditorGUILayout.PropertyField( m_HorizontalScrollbarSpacingProperty, EditorGUIUtility.TrTextContent( "Spacing"));
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField( m_VerticalScrollbarProperty);
			
            if( m_VerticalScrollbarProperty.objectReferenceValue != null
			&&	m_VerticalScrollbarProperty.hasMultipleDifferentValues == false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField( m_VerticalScrollbarVisibilityProperty, EditorGUIUtility.TrTextContent( "Visibility"));
				
                if( (ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibilityProperty.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                &&	m_VerticalScrollbarVisibilityProperty.hasMultipleDifferentValues == false)
                {
                    if( m_ViewportIsNotChild != false || m_VScrollbarIsNotChild != false)
					{
                        EditorGUILayout.HelpBox( s_VError, MessageType.Error);
					}
                    EditorGUILayout.PropertyField( m_VerticalScrollbarSpacingProperty, EditorGUIUtility.TrTextContent( "Spacing"));
                }
                EditorGUI.indentLevel--;
            }
			EditorGUILayout.Space();
            EditorGUILayout.PropertyField( m_OnValueChangedProperty);
			
			serializedObject.ApplyModifiedProperties();
		}
		SerializedProperty m_ContentProperty;
		SerializedProperty m_HorizontalProperty;
		SerializedProperty m_VerticalProperty;
		SerializedProperty m_MovementTypeProperty;
		SerializedProperty m_ElasticityProperty;
		SerializedProperty m_InertiaProperty;
		SerializedProperty m_DecelerationRateProperty;
		SerializedProperty m_ScrollSensitivityProperty;
		SerializedProperty m_ViewportProperty;
		SerializedProperty m_HorizontalScrollbarProperty;
		SerializedProperty m_VerticalScrollbarProperty;
		SerializedProperty m_HorizontalScrollbarVisibilityProperty;
		SerializedProperty m_VerticalScrollbarVisibilityProperty;
		SerializedProperty m_HorizontalScrollbarSpacingProperty;
		SerializedProperty m_VerticalScrollbarSpacingProperty;
		SerializedProperty m_OnValueChangedProperty;
		AnimBool m_ShowElasticity;
		AnimBool m_ShowDecelerationRate;
		bool m_ViewportIsNotChild;
		bool m_HScrollbarIsNotChild;
		bool m_VScrollbarIsNotChild;
		static readonly string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
		static readonly string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
	}
}