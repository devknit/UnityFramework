
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( Toggle), true), CanEditMultipleObjects]
	public class ToggleEditor : SelectableEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			m_ToggleGraphicProperty = PopProperty( "m_ToggleGraphic");
			m_OverrideSpriteProperty = PopProperty( "m_OverrideSprite");
			m_ToggleGroupProperty = PopProperty( "m_ToggleGroup");
			m_AwakeOnEventProperty = PopProperty( "m_AwakeOnEvent");
			m_IsOnProperty = PopProperty( "m_IsOn");
			m_OnValueChangedProperty = PopProperty( "m_OnValueChanged");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			OnModuleGUI();
			OnStatusGUI();
			OnGraphicGUI();
			EditorGUILayout.PropertyField( m_ToggleGraphicProperty);
			EditorGUILayout.PropertyField( m_OverrideSpriteProperty);
			OnNavigationGUI();
			OnActionGUI();
			EditorGUILayout.PropertyField( m_ToggleGroupProperty);
			if( m_ToggleGroupProperty.objectReferenceValue == null)
			{
				EditorGUILayout.PropertyField( m_AwakeOnEventProperty);
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( m_IsOnProperty);
			bool applyToggle = EditorGUI.EndChangeCheck();
			OnEventGUI();
			EditorGUILayout.PropertyField( m_OnValueChangedProperty);
			OnOtherGUI();
			serializedObject.ApplyModifiedProperties();
			
			if( applyToggle != false && target is Toggle toggle)
			{
				toggle.ApplyToggle();
			}
		}
		SerializedProperty m_ToggleGraphicProperty;
		SerializedProperty m_OverrideSpriteProperty;
		SerializedProperty m_ToggleGroupProperty;
		SerializedProperty m_AwakeOnEventProperty;
		SerializedProperty m_IsOnProperty;
		SerializedProperty m_OnValueChangedProperty;
	}
}