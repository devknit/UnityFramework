
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.IO;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Knit.Framework.Sample
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public sealed class EntryPoint : MonoBehaviour
	{
	#if UNITY_EDITOR
		static readonly string[] kAssetGUIDs = 
		{
			"19b98b477e0d4a143bc084925f589c75",
			"92bbf1be9afda1041b897e2c2aac2c87",
			"300732e2a8462754fb24994134feed3f",
			"d5918581b5a7e7147a3bb93b78966952",
			"abf7f90f961eb644b9661f0f7667fdad",
			"b8533b4616d681147a2e126e4d691c56",
		};
		static EntryPoint()
		{
			EditorApplication.playModeStateChanged += OnChangedPlayMode;
		}
		static void OnChangedPlayMode( PlayModeStateChange state)
		{
			if( state == PlayModeStateChange.ExitingEditMode)
			{
				AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
				AddressableAssetGroup defaultGroup = settings.DefaultGroup;
				
				foreach( var assetGUID in kAssetGUIDs)
				{
					string assetPath = AssetDatabase.GUIDToAssetPath( assetGUID);
					
					if( string.IsNullOrEmpty( assetPath) == false)
					{
						string assetName = Path.GetFileNameWithoutExtension( assetPath);
						
						if( string.IsNullOrEmpty( assetName) == false && settings.FindAssetEntry( assetGUID) == null)
						{
							settings.CreateOrMoveEntry( assetGUID, defaultGroup)?.SetAddress( assetName);
						}
					}
				}
			}
		}
	#endif
		IEnumerator Start()
		{
			SceneInstance? sceneInstance = null;
			Color fadeColor = m_Fade.color;
			const float duration = 1.0f;
			float elapsedTime = 0.0f;
			
			SceneHandle.Boot( m_BootScene).Completed += (handle) =>
			{
				sceneInstance = handle.Result;
			};
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				fadeColor.a = 1.0f - Mathf.Clamp01( elapsedTime / duration);
				m_Fade.color = fadeColor;
				yield return null;
			}
			fadeColor.a = 0.0f;
			elapsedTime = 0.0f;
			
			while( sceneInstance.HasValue == false || elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			elapsedTime = 0.0f;
			
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				fadeColor.a = Mathf.Clamp01( elapsedTime / duration);
				m_Fade.color = fadeColor;
				yield return null;
			}
			fadeColor.a = 1.0f;
			
			yield return sceneInstance.Value.ActivateAsync();
		}
		[SerializeField]
		AssetReferenceScene m_BootScene;
		[SerializeField]
		Image m_Fade;
	}
}