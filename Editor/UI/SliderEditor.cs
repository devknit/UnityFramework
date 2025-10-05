
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( Slider), true), CanEditMultipleObjects]
	public class SliderEditor : SelectableEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			m_DirectionProperty = PopProperty( "m_Direction");
			m_FillRectProperty = PopProperty( "m_FillRect");
			m_HandleRectProperty = PopProperty( "m_HandleRect");
			m_ValueProperty = PopProperty( "m_Value");
			m_MinValueProperty = PopProperty( "m_MinValue");
			m_MaxValueProperty = PopProperty( "m_MaxValue");
			m_WholeNumbersProperty = PopProperty( "m_WholeNumbers");
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
			EditorGUILayout.PropertyField( m_DirectionProperty);
			if( EditorGUI.EndChangeCheck() != false)
			{
				Undo.RecordObjects( serializedObject.targetObjects, "Change Slider Direction");
				
				foreach( var slider in serializedObject.targetObjects.Cast<Slider>())
				{
					slider.SetDirection( (Slider.Direction)m_DirectionProperty.enumValueIndex);
				}
			}
			EditorGUILayout.PropertyField( m_FillRectProperty);
			EditorGUILayout.PropertyField( m_HandleRectProperty);
			EditorGUI.BeginChangeCheck();
			float minValue = EditorGUILayout.FloatField( m_MinValueProperty.displayName, m_MinValueProperty.floatValue);
			if( EditorGUI.EndChangeCheck() != false)
			{
				if( m_WholeNumbersProperty.boolValue != false)
				{
					minValue = Mathf.Round( minValue);
				}
				if( minValue < m_MaxValueProperty.floatValue)
				{
					m_MinValueProperty.floatValue = minValue;
					
					if (m_ValueProperty.floatValue < minValue)
					{
						m_ValueProperty.floatValue = minValue;
						serializedObject.ApplyModifiedProperties();
						
						foreach( var slider in serializedObject.targetObjects.Cast<Slider>())
						{
							slider.SetFromEditor( m_ValueProperty.floatValue);
						}
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			float maxValue = EditorGUILayout.FloatField( m_MaxValueProperty.displayName, m_MaxValueProperty.floatValue);
			if( EditorGUI.EndChangeCheck() != false)
			{
				if( m_WholeNumbersProperty.boolValue != false)
				{
					maxValue = Mathf.Round( maxValue);
				}
				if( maxValue > m_MinValueProperty.floatValue)
				{
					m_MaxValueProperty.floatValue = maxValue;
					
					if( m_ValueProperty.floatValue > maxValue)
					{
						m_ValueProperty.floatValue = maxValue;
						serializedObject.ApplyModifiedProperties();
						
						foreach( var slider in serializedObject.targetObjects.Cast<Slider>())
						{
							slider.SetFromEditor( m_ValueProperty.floatValue);
						}
					}
				}
			}
			float value;
			
			using( new EditorGUI.DisabledGroupScope( m_MinValueProperty.floatValue == m_MaxValueProperty.floatValue))
			{
				EditorGUI.BeginChangeCheck();
				value = EditorGUILayout.Slider( m_ValueProperty.displayName, 
					m_ValueProperty.floatValue, m_MinValueProperty.floatValue, m_MaxValueProperty.floatValue);
				if( EditorGUI.EndChangeCheck() != false)
				{
					if( m_WholeNumbersProperty.boolValue != false)
					{
						value = Mathf.Round( value);
					}
					m_ValueProperty.floatValue = value;
					serializedObject.ApplyModifiedProperties();
					
					foreach( var slider in serializedObject.targetObjects.Cast<Slider>())
					{
						slider.SetFromEditor( m_ValueProperty.floatValue);
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( m_WholeNumbersProperty);
			if( EditorGUI.EndChangeCheck() != false)
			{
				if( m_WholeNumbersProperty.boolValue != false)
				{
					value = Mathf.Round( value);
					minValue = Mathf.Round( minValue);
					maxValue = Mathf.Round( maxValue);
					m_ValueProperty.floatValue = value;
					m_MinValueProperty.floatValue = minValue;
					m_MaxValueProperty.floatValue = maxValue;
					serializedObject.ApplyModifiedProperties();
					
					foreach( var slider in serializedObject.targetObjects.Cast<Slider>())
					{
						slider.SetFromEditor( m_ValueProperty.floatValue);
					}
				}
			}
			OnEventGUI();
			EditorGUILayout.PropertyField( m_OnValueChangedProperty);
			OnOtherGUI();
			serializedObject.ApplyModifiedProperties();
		}
		SerializedProperty m_DirectionProperty;
		SerializedProperty m_FillRectProperty;
		SerializedProperty m_HandleRectProperty;
		SerializedProperty m_ValueProperty;
		SerializedProperty m_MinValueProperty;
		SerializedProperty m_MaxValueProperty;
		SerializedProperty m_WholeNumbersProperty;
		SerializedProperty m_OnValueChangedProperty;
	}
}