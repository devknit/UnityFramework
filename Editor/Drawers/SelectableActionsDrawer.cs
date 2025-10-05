
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomPropertyDrawer( typeof( SelectableActionsAttribute))]
	sealed class SelectableActionsDrawer : PropertyDrawer 
	{
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			if( property.propertyType == SerializedPropertyType.Integer
			&&	property.serializedObject.targetObject is Selectable selectable)
			{
				var setting = selectable.m_ModuleSetting;
				
				if( setting != null)
				{
					var displayedOptions = setting.SelectableActions.Prepend( null)
						.Select( x => new GUIContent( x?.ActionName ?? "None")).ToArray();
					EditorGUI.BeginChangeCheck();
					int value = EditorGUI.Popup( position, label, property.intValue + 1, displayedOptions);
					if( EditorGUI.EndChangeCheck() != false)
					{
						property.intValue = value - 1;
					}
					return;
				}
			}
			EditorGUI.PropertyField( position, property, true);
		}
	}
}