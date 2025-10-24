
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( ToggleGroup), true), CanEditMultipleObjects]
	public class ToggleGroupEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			
			SerializedProperty serializedProperty = serializedObject.GetIterator();
			
			serializedProperty.NextVisible( true);
			
			while( serializedProperty.NextVisible( false) != false)
			{
				if( serializedProperty.name == "m_Script")
				{
					continue;
				}
				EditorGUILayout.PropertyField( serializedProperty);
			}
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			
			using( new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				
				if( GUILayout.Button( "Collection from Children") != false)
				{
					foreach( var target in targets)
					{
						if( target is ToggleGroup toggleGroup)
						{
							toggleGroup.CollectionsInChildren();
							EditorUtility.SetDirty( toggleGroup);
						}
					}
				}
				var dropDownToggleButtonStyle = typeof( EditorStyles).GetProperty( 
					"dropDownToggleButton", BindingFlags.Static | BindingFlags.NonPublic).GetValue( null) as GUIStyle;
				var content = new GUIContent( "Apply on Children");
				
				Rect rect = GUILayoutUtility.GetRect( content, dropDownToggleButtonStyle, GUILayout.Width( 160));
				Rect rectPopupButton = rect;
				rectPopupButton.x += rect.width - 16;
				rectPopupButton.width = 16;
				
				if( EditorGUI.DropdownButton( rectPopupButton, GUIContent.none, FocusType.Passive, GUIStyle.none) != false)
				{
					var menu = new GenericMenu();
					
					menu.AddItem( new GUIContent( "Override"), false, () =>
					{
						Apply( targets, true);
					});
					menu.AddItem(new GUIContent( "Only to None"), false, () =>
					{
						Apply( targets, false);
					});
					menu.DropDown( rect);
				}
				else if( GUI.Button( rect, content, dropDownToggleButtonStyle))
				{
					Apply( targets, true);
					GUIUtility.ExitGUI();
				}
			}
		}
		static void Apply( Object[] targets, bool force)
		{
			foreach( var target in targets)
			{
				if( target is ToggleGroup toggleGroup)
				{
					var toggles = toggleGroup.GetComponentsInChildren<Toggle>( true);
					
					for( int i0 = 0; i0 < toggles.Length; ++i0)
					{
						var toggle = toggles[ i0];
						
						if( toggle.ToggleGroup == null || force != false)
						{
							toggle.ToggleGroup = toggleGroup;
							EditorUtility.SetDirty( toggle);
						}
					}
				}
			}
		}
	}
}