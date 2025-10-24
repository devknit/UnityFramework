
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Knit.Framework.Editor
{
	public sealed class SceneUtility
	{
		public static void ApplyScene( Func<string, bool> leaveAsSceneCallback,  params string[] sceneLocates)
		{
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			
			if( sceneLocates != null && settings != null)
			{
				for( int i0 = EditorSceneManager.sceneCount - 1; i0 >= 0; --i0)
				{
					Scene scene = EditorSceneManager.GetSceneAt( i0);
					var guid = AssetDatabase.AssetPathToGUID( scene.path);
					var entry = settings.FindAssetEntry(guid);
					
					if( entry != null)
					{
						var matchLocate = sceneLocates.Where( x => x == entry.address);
						
						if( matchLocate.Count() == 0)
						{
							if( (leaveAsSceneCallback?.Invoke( scene.name) ?? false) == false)
							{
								EditorSceneManager.CloseScene( scene, true);
							}
						}
					}
				}
				string[] guids = AssetDatabase.FindAssets( "t:Scene");
				bool setActive = false;
				
				for( int i0 = 0; i0 < guids.Length; ++i0)
				{
					var entry = settings.FindAssetEntry( guids[ i0]);
					
					if( entry != null)
					{
						for( int i1 = 0; i1 < sceneLocates.Length; ++i1)
						{
							if( entry.address == sceneLocates[ i1])
							{
								bool sceneOpened = false;
								string sceneAssetPath = AssetDatabase.GUIDToAssetPath( guids[ i0]);
								Scene scene = default;
								
								for( int i2 = 0; i2 < EditorSceneManager.sceneCount; ++i2)
								{
									scene = EditorSceneManager.GetSceneAt( i2);
									
									if( scene.path == sceneAssetPath)
									{
										sceneOpened = true;
										break;
									}
								}
								if( sceneOpened == false)
								{
									scene = EditorSceneManager.OpenScene( sceneAssetPath, OpenSceneMode.Additive);
								}
								if( scene.IsValid() != false)
								{                                    
									if( setActive == false)
									{
										EditorSceneManager.SetActiveScene( scene);
									}
									for( int i2 = 0; i2 < scene.rootCount; ++i2)
									{
										scene.GetRootGameObjects()[ i2].SetActive( setActive == false);
									}
									setActive = true;
								}
								break;
							}
						}
					}
				}
			}
		}
		public static List<string> GetScene( SceneNoun noun)
		{
			var results = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:Scene");
			
			switch( noun)
			{
				case SceneNoun.Locate:
				{
					var settings = AddressableAssetSettingsDefaultObject.Settings;
					
					if( settings != null)
					{
						for( int i0 = 0; i0 < guids.Length; ++i0)
						{
							var entry = settings.FindAssetEntry( guids[ i0]);
							
							if( entry != null)
							{
								results.Add( entry.address);
							}
						}
					}
					break;
				}
				case SceneNoun.Path:
				{
					for( int i0 = 0; i0 < guids.Length; ++i0)
					{
						results.Add( AssetDatabase.GUIDToAssetPath( guids[ i0]));
					}
					break;
				}
				case SceneNoun.Name:
				{
					for( int i0 = 0; i0 < guids.Length; ++i0)
					{
						var path = AssetDatabase.GUIDToAssetPath( guids[ i0]);
						results.Add( Path.GetFileNameWithoutExtension( path));
					}
					break;
				}
			}
			return results;
		}
	}
}