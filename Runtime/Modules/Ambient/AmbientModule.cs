
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;

namespace Knit.Framework
{
	public class AmbientModule : MonoBehaviour
	{
		public static bool Contains( string sceneName)
		{
			return s_Caches.ContainsKey( sceneName);
		}
		public static bool Forcus( string sceneName)
		{
			if( s_Caches.TryGetValue( sceneName, out AmbientCahce target) != false)
			{
				if( target.Forcus( true) != false)
				{
					foreach( var cache in s_Caches.Values)
					{
						if( cache != target)
						{
							cache.Forcus( false);
						}
					}
					return true;
				}
			}
			return false;
		}
		public static string GetForcus()
		{
			foreach( var handle in s_Caches)
			{
				if( handle.Value.GetForcusPriority() == 0)
				{
					return handle.Key;
				}
			}
			return string.Empty;
		}
		public static bool TryGetTransform( string sceneName, out Transform transform)
		{
			if( s_Caches.TryGetValue( sceneName, out AmbientCahce target) != false)
			{
				transform = target?.Module?.transform;
				return true;
			}
			transform = null;
			return false;
		}
		internal static bool LoadAsync( string sceneName)
		{
			if( string.IsNullOrEmpty( sceneName) == false)
			{
				if( s_Caches.TryGetValue( sceneName, out AmbientCahce cache) != false)
				{
					cache.Reference();
					return true;
				}
				else
				{
					AsyncOperationHandle<SceneInstance> handle = 
						Addressables.LoadSceneAsync( sceneName, new LoadSceneParameters( LoadSceneMode.Additive), false, 100);
					if( handle.IsValid() != false)
					{
						cache = new AmbientCahce( handle, s_Caches.Count + 1);
						s_Caches.Add( sceneName, cache);
						
						handle.Completed += (handle) =>
						{
							if( handle.Status == AsyncOperationStatus.Succeeded)
							{
								handle.Result.ActivateAsync().completed += (op) =>
								{
									switch( cache.Activated())
									{
										case AmbientActivate.Error:
										{
											Release( sceneName);
											break;
										}
										case AmbientActivate.Forcus:
										{
											Forcus( sceneName);
											break;
										}
									}
								};
							}
							else
							{
								Release( sceneName);
							}
						};
						return true;
					}
				}
			}
			return false;
		}
		internal static bool Release( string sceneName)
		{
			if( s_Caches.TryGetValue( sceneName, out AmbientCahce target) != false)
			{
				switch( target.Release())
				{
					case AmbientRelease.Keep:
					{
						return true;
					}
					case AmbientRelease.Unload:
					{
						s_Caches.Remove( sceneName);
						return true;
					}
					case AmbientRelease.ForcusUnload:
					{
						s_Caches.Remove( sceneName);
						
						AmbientCahce nextTarget = null;
						
						foreach( var cache in s_Caches.Values)
						{
							if( (nextTarget?.Priority ?? int.MaxValue) > cache.Priority)
							{
								nextTarget = cache;
							}
						}
						if( (nextTarget?.Forcus( true) ?? false) != false)
						{
							foreach( var cache in s_Caches.Values)
							{
								if( cache != nextTarget)
								{
									cache.Forcus( false);
								}
							}
						}
						return true;
					}
				}
			}
			return false;
		}
		internal static bool IsLoading()
		{
			foreach( var handle in s_Caches.Values)
			{
				if( handle.IsLoading() != false)
				{
					return true;
				}
			}
			return false;
		}
		void Awake()
		{
			if( s_Caches.TryGetValue( name, out AmbientCahce cache) != false)
			{
				cache.SetAmbientHandle( this);
			}
			else
			{
				s_Caches.Add( name, new AmbientCahce( this, s_Caches.Count + 1));
			}
		}
		void OnDestroy()
		{
			Release( name);
		}
	#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void OnEnterPlaymodeInEditor()
		{
			UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}
		static void OnPlayModeStateChanged( UnityEditor.PlayModeStateChange state)
		{
		#if true
			if( state == UnityEditor.PlayModeStateChange.ExitingEditMode)
			{
				string activeScenePath = SceneManager.GetActiveScene().path;
				var removeScenePaths = new List<string>();
				
				for( int i0 = SceneManager.sceneCount - 1; i0 >= 0; --i0)
				{
					Scene scene = SceneManager.GetSceneAt( i0);
					var gameObjects = scene.GetRootGameObjects();
					
					for( int i1 = 0; i1 < gameObjects.Length; ++i1)
					{
						if( gameObjects[ i1].GetComponent<AmbientModule>() != false)
						{
							removeScenePaths.Add( scene.path);
							UnityEditor.SceneManagement.EditorSceneManager.CloseScene( scene, true);
							break;
						}
					}
				}
				if( removeScenePaths.Count > 0)
				{
					UnityEditor.SessionState.SetString( kSeccionStateActiveSceneKey, activeScenePath);
					UnityEditor.SessionState.SetString( kSeccionStateRemoveScenesKey, string.Join( '\n', removeScenePaths));
				}
			}
			if( state == UnityEditor.PlayModeStateChange.EnteredEditMode)
			{
				string activeScenePath = UnityEditor.SessionState.GetString( kSeccionStateActiveSceneKey, string.Empty);
				string removeScenePaths = UnityEditor.SessionState.GetString( kSeccionStateRemoveScenesKey, string.Empty);
				
				if( string.IsNullOrWhiteSpace( removeScenePaths) == false)
				{
					string[] removeScenePath = removeScenePaths.Split( '\n');
					
					for( int i0 = 0; i0 < removeScenePath.Length; ++i0)
					{
						if( string.IsNullOrEmpty( removeScenePath[ i0]) == false)
						{
							var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene( 
								removeScenePath[ i0], UnityEditor.SceneManagement.OpenSceneMode.Additive);
							
							if( removeScenePath[ i0] == activeScenePath)
							{
								SceneManager.SetActiveScene( scene);
							}
						}
					}
				}
				UnityEditor.SessionState.SetString( kSeccionStateActiveSceneKey, string.Empty);
				UnityEditor.SessionState.SetString( kSeccionStateRemoveScenesKey, string.Empty);
			}
		#else
			if( state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
			{
				for( int i0 = SceneManager.sceneCount - 1; i0 >= 0; --i0)
				{
					Scene scene = SceneManager.GetSceneAt( i0);
					var gameObjects = scene.GetRootGameObjects();
					
					for( int i1 = 0; i1 < gameObjects.Length; ++i1)
					{
						if( gameObjects[ i1].activeSelf == false)
						{
							gameObjects[ i1].SetActive( true);
							gameObjects[ i1].SetActive( false);
						}
					}
				}
			}
		#endif
		}
		const string kSeccionStateActiveSceneKey = "AmbientModule.ActiveScene";
		const string kSeccionStateRemoveScenesKey = "AmbientModule.RemoveScenes";
	#endif
		static readonly Dictionary<string, AmbientCahce> s_Caches = new();
	}
}