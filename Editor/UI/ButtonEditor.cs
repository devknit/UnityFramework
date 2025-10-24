
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( Button), true), CanEditMultipleObjects]
	public class ButtonEditor : SelectableEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			m_ClickModeProperty = PopProperty( "m_ClickMode");
			m_OnClickProperty = PopProperty( "m_OnClick");
			m_OnLongPressProperty = PopProperty( "m_OnLongPress");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			OnModuleGUI();
			OnStatusGUI();
			OnGraphicGUI();
			OnNavigationGUI();
			OnActionGUI();
			EditorGUILayout.PropertyField( m_ClickModeProperty);
			OnEventGUI();
			EditorGUILayout.PropertyField( m_OnClickProperty);
			EditorGUILayout.PropertyField( m_OnLongPressProperty);
			OnOtherGUI();
			serializedObject.ApplyModifiedProperties();
		}
		SerializedProperty m_ClickModeProperty;
		SerializedProperty m_OnClickProperty;
		SerializedProperty m_OnLongPressProperty;
	}
}