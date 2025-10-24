
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomPropertyDrawer( typeof( SceneSelectorAttribute))]
	public class SceneSelectorDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			var sceneSelector = attribute as SceneSelectorAttribute;
			GUIContent[] displayNames = SceneUtility.GetScene( sceneSelector.Noun)
				.Prepend( kNone).Select( x => new GUIContent( x)).ToArray();
			int selectIndex = Mathf.Max( 0, Array.FindIndex( displayNames, ( x) => x.text == property.stringValue));
			
			selectIndex = EditorGUI.Popup( position, label, selectIndex, displayNames);
			
			if( EditorGUI.EndChangeCheck() != false)
			{
				property.stringValue = displayNames[ selectIndex].text;
			}
		}
		const string kNone = "None";
	}
}