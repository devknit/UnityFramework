
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( Scrollbar), true), CanEditMultipleObjects]
	public class ScrollbarEditor : SelectableEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			m_HandleRectProperty = PopProperty( "m_HandleRect");
			m_DirectionProperty = PopProperty( "m_Direction");
			m_ValueProperty = PopProperty( "m_Value");
			m_SizeProperty = PopProperty( "m_Size");
			m_NumberOfStepsProperty = PopProperty( "m_NumberOfSteps");
			m_OnValueChangedProperty = PopProperty( "m_OnValueChanged");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			OnModuleGUI();
			OnStatusGUI();
			OnGraphicGUI();
			OnNavigationGUI();
			OnActionGUI();
			
			EditorGUI.BeginChangeCheck();
			RectTransform handleRect = EditorGUILayout.ObjectField( m_HandleRectProperty.displayName, 
				m_HandleRectProperty.objectReferenceValue, typeof( RectTransform), true) as RectTransform;
			if( EditorGUI.EndChangeCheck() != false)
			{
				var modifiedObjects = new List<Object>()
				{
					handleRect
				};
				foreach( var target in m_HandleRectProperty.serializedObject.targetObjects)
				{
					if( target is MonoBehaviour monoBehaviour)
					{
						modifiedObjects.Add( monoBehaviour);
						modifiedObjects.Add( monoBehaviour.GetComponent<RectTransform>());
					}
				}
				Undo.RecordObjects( modifiedObjects.ToArray(), "Change Handle Rect");
				m_HandleRectProperty.objectReferenceValue = handleRect;
			}
			if( m_HandleRectProperty.objectReferenceValue != null)
			{
				bool bUpdate = false;
				bool bWarning = false;
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( m_DirectionProperty);
				if( EditorGUI.EndChangeCheck() != false)
				{
					Undo.RecordObjects( serializedObject.targetObjects, "Change Scrollbar Direction");
					Scrollbar.Direction direction = (Scrollbar.Direction)m_DirectionProperty.enumValueIndex;
					
					foreach( var scrollbar in serializedObject.targetObjects.Cast<Scrollbar>())
					{
						scrollbar.SetDirection( direction, true);
					}
					bUpdate = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( m_ValueProperty);
				if( EditorGUI.EndChangeCheck() != false)
				{
					bUpdate = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( m_SizeProperty);
				if( EditorGUI.EndChangeCheck() != false)
				{
					bUpdate = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( m_NumberOfStepsProperty);
				if( EditorGUI.EndChangeCheck() != false)
				{
					bUpdate = true;
				}
				foreach( var scrollbar in serializedObject.targetObjects.Cast<Scrollbar>())
				{
					Scrollbar.Direction direction = scrollbar.ScrollDirection;
					
					if( direction == Scrollbar.Direction.LeftToRight || direction == Scrollbar.Direction.RightToLeft)
					{
						bWarning = scrollbar.HasHorizontalNavigate() == false && (scrollbar.FindSelectableOnLeft() != null || scrollbar.FindSelectableOnRight() != null);
					}
					else
					{
						bWarning = scrollbar.HasVerticalNavigate() == false && (scrollbar.FindSelectableOnDown() != null || scrollbar.FindSelectableOnUp() != null);
					}
				}
				if( bUpdate != false)
				{
					foreach( var scrollbar in serializedObject.targetObjects.Cast<Scrollbar>())
					{
						scrollbar.Update();
					}
				}
				if( bWarning != false)
				{
					EditorGUILayout.HelpBox( "The selected scrollbar direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);
				}
			}
			OnEventGUI();
			EditorGUILayout.PropertyField( m_OnValueChangedProperty);
			OnOtherGUI();
			serializedObject.ApplyModifiedProperties();
		}
		SerializedProperty m_HandleRectProperty;
		SerializedProperty m_DirectionProperty;
		SerializedProperty m_ValueProperty;
		SerializedProperty m_SizeProperty;
		SerializedProperty m_NumberOfStepsProperty;
		SerializedProperty m_OnValueChangedProperty;
	}
}