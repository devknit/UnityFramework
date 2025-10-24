
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Knit.Framework.Editor
{
	[CustomPropertyDrawer( typeof( SceneLocates))]
	sealed class SceneLocatesDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative( "m_Values");
			float height = EditorGUI.GetPropertyHeight( valueProperty) + EditorGUIUtility.standardVerticalSpacing;
			
			if( valueProperty.isExpanded != false)
			{
				var component = property.serializedObject.targetObject as Component;
				
				if( (component?.gameObject.scene.IsValid() ?? false) != false)
				{
					height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
				}
			}
			return height;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative( "m_Values");
			position.height = EditorGUI.GetPropertyHeight( valueProperty);
			EditorGUI.PropertyField( position, valueProperty, new GUIContent( property.displayName));
			
			if( valueProperty.isExpanded != false)
			{
				var component = property.serializedObject.targetObject as Component;
				
				if( (component?.gameObject.scene.IsValid() ?? false) != false &&
					PrefabStageUtility.GetCurrentPrefabStage() == null)
				{
					position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
					position.height = EditorGUIUtility.singleLineHeight;
					position.xMin = position.xMax - GUI.skin.button.CalcSize( kButtonContent).x;;
					
					if( GUI.Button( position, kButtonContent) != false)
					{
						var sceneLocates = property.boxedValue as SceneLocates; 
						SceneUtility.ApplyScene( (sceneName) => {
							if( sceneName == component?.gameObject.scene.name)
							{
								return true;
							}
							return false;
						}, sceneLocates.Values);
					}
				}
			}
		}
		static readonly GUIContent kButtonContent = new( "Apply");
	}
}