
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Knit.Framework.Editor
{
	[CustomPropertyDrawer( typeof( SceneLocate))]
	public class SceneLocateDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative( "m_Value");
			var component = property.serializedObject.targetObject as Component;
			float buttonWidth = 0.0f;
			
			if( (component?.gameObject.scene.IsValid() ?? false) != false && 
				PrefabStageUtility.GetCurrentPrefabStage() == null)
			{
				buttonWidth = GUI.skin.button.CalcSize( kButtonContent).x;
			}
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField( position, property.displayName);
			
			position.xMin += EditorGUIUtility.labelWidth;
			position.width = position.width - buttonWidth;
			EditorGUI.PropertyField( position, valueProperty);
			
			if( buttonWidth > 0.0f)
			{
				position.xMin += position.width;
				position.width = buttonWidth;
				
				if( GUI.Button( position, kButtonContent) != false)
				{
					var locate = property.boxedValue as SceneLocate;
					
					SceneUtility.ApplyScene( (sceneName) => {
						if( sceneName == component?.gameObject.scene.name)
						{
							return true;
						}
						return false;
					}, locate.Value);
				}
			}
		}
		static readonly GUIContent kButtonContent = new( "Apply");
	}
}