
using UnityEngine;
using UnityEditor;

namespace Knit.Framework.Editor
{
	[CustomEditor( typeof( AmbientModule), true)]
	public class AmbientModuleEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			using( new GUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				
				using( new EditorGUI.DisabledGroupScope( Application.isPlaying == false))
				{
					if( GUILayout.Button( "Active") != false)
					{
						AmbientModule.Forcus( target.name);
					}
				}
			}
		}
	}
}