
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace Knit.Framework
{
	public abstract partial class SceneHandle : ModuleHandle
	{
		protected virtual void Awake()
		{
			if( m_BaseCamera != null)
			{
				float depth = 0;
				
				m_BaseCamera.enabled = false;
				m_BaseCamera.depth = depth++;
				
				var cameraStack = m_BaseCamera.GetUniversalAdditionalCameraData().cameraStack;
				
				for( int i0 = 0; i0 < cameraStack.Count; ++i0)
				{
					cameraStack[ i0].depth = depth++;
				}
				m_CameraDepthOffset = depth;
			}
		}
		IEnumerator Start()
		{
			bool bSuccess = true;
			IEnumerator it;
			
			if( m_ModuleSetting != null)
			{
				it = m_ModuleSetting.Initialize();
				
				while( it.MoveNext() != false)
				{
					yield return it.Current;
				}
			}
			else
			{
			#if UNITY_EDITOR
				Debug.LogError( "ModuleSetting が設定されていません");
			#endif
				if( s_CurrentHandle != null)
				{
					s_CurrentHandle.StartCoroutine( 
						s_CurrentHandle.Resume( m_SceneContext));
				}
				it = Release();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				yield break;
			}
			if( ModuleDaemon.Instance != null)
			{
				while( ModuleDaemon.Instance.IsInitialized == false)
				{
					yield return null;
				}
			}
			if( s_ModuleHandle != null)
			{
				m_ModuleHandle = s_ModuleHandle;
				s_ModuleHandle = null;
			}
			SetModuleContext( m_SceneContext = OnSceneContext( s_CurrentHandle?.m_RequestContext), this);
			
			if( m_SceneContext == null)
			{
				if( s_CurrentHandle != null)
				{
					s_CurrentHandle.StartCoroutine( 
						s_CurrentHandle.Resume( m_SceneContext));
				}
				it = Release();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				yield break;
			}
			if( m_SceneContext.IsModuleState( ModuleState.PreLoaded) == false)
			{
				it = m_SceneContext.OnPreLoad();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_SceneContext.AddModuleState( ModuleState.PreLoaded);
			}
			m_SceneContext.AddModuleState( ModuleState.Instanced);
			
			if( m_SceneContext.IsModuleState( ModuleState.ContextPrepare) == false)
			{
				it = m_SceneContext.OnPrepare( () => bSuccess = false);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					if( s_CurrentHandle != null)
					{
						s_CurrentHandle.StartCoroutine( s_CurrentHandle.Resume( m_SceneContext));
					}
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_SceneContext.AddModuleState( ModuleState.ContextPrepare);
			}
			if( m_SceneContext.IsModuleState( ModuleState.ModulePrepare) == false)
			{
				it = OnPrepare( () => bSuccess = false);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					if( s_CurrentHandle != null)
					{
						s_CurrentHandle.StartCoroutine( s_CurrentHandle.Resume( m_SceneContext));
					}
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_SceneContext.AddModuleState( ModuleState.ModulePrepare);
			}
			if( m_SceneContext.IsModuleState( ModuleState.Loaded) == false)
			{
				while( m_SceneContext.IsLoading() != false)
				{
					yield return null;
				}
				if( m_SceneContext.IsLoadFailed() != false)
				{
					if( s_CurrentHandle != null)
					{
						s_CurrentHandle.StartCoroutine( 
							s_CurrentHandle.Resume( m_SceneContext));
					}
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_SceneContext.AddModuleState( ModuleState.Loaded);
			}
			if( m_SceneContext.IsModuleState( ModuleState.Setupd) == false)
			{
				it = OnSetup( () => bSuccess = false );
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					if( s_CurrentHandle != null)
					{
						s_CurrentHandle.StartCoroutine( 
							s_CurrentHandle.Resume( m_SceneContext));
					}
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				if( s_CurrentHandle != null)
				{
					StartCoroutine( s_CurrentHandle.Release());
				}
				if( m_BaseCamera != null)
				{
					m_BaseCamera.enabled = true;
				}
				ModuleDaemon.Instance?.PushOverlayCamera( m_BaseCamera);
				s_CurrentHandle = this;
				m_SceneContext.AddModuleState( ModuleState.Setupd);
			}
			if( m_SceneContext.IsModuleState( ModuleState.Opened) == false)
			{
				it = OnOpen();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_SceneContext.AddModuleState( ModuleState.Opened);
			}
			if( m_SceneContext.IsModuleState( ModuleState.Started) == false)
			{
				it = OnStart();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_SceneContext.AddModuleState( ModuleState.Started);
			}
			FocusIn();
		}
		IEnumerator Stop( SceneContext sceneContext, bool bAllowOnlyEquivalent)
		{
			IEnumerator it;
			
			if( m_SceneContext.IsModuleState( ModuleState.Focus) != false)
			{
				m_SceneContext.RemoveModuleState( ModuleState.Focus);
				m_OnModuleSateFocus?.Invoke( false);
				OnFocusOut();
			}
			if( s_UnloadSceneCount > 0)
			{
			#if UNITY_EDITOR
				Debug.LogError( "以前のシーンが解放されていません");
			#endif
				it = Resume( sceneContext);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				yield break;
			}
			if( bAllowOnlyEquivalent != false)
			{
				if( m_SceneContext.SceneName != sceneContext.SceneName)
				{
				#if UNITY_EDITOR
					Debug.LogError( $"異なるコンテキスト名です。Current<{m_SceneContext.SceneName}> != Request<{sceneContext.SceneName}>");
				#endif
					it = Resume( sceneContext);
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				if( m_SceneContext.GetType() != sceneContext.GetType())
				{
				#if UNITY_EDITOR
					Debug.LogError( $"異なるコンテキスト型です。Current<{m_SceneContext.GetType()}> != Request<{sceneContext.GetType()}>");
				#endif
					it = Resume( sceneContext);
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
			}
			if( m_SceneContext.IsModuleState( ModuleState.Started) != false)
			{
				bool success = true;
				
				m_SceneContext.RemoveModuleState( ModuleState.Started);
				it = OnStop( sceneContext, () => success = false);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( success == false)
				{
					it = Resume( sceneContext);
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
			}
			if( m_SceneContext.IsModuleState( ModuleState.Opened) != false)
			{
				m_SceneContext.RemoveModuleState( ModuleState.Opened);
				it = OnClose();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
			}
			if( sceneContext != null)
			{
				it = sceneContext.OnPreLoad();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				sceneContext.AddModuleState( ModuleState.PreLoaded);
				m_RequestContext = sceneContext;
				
				s_ModuleHandle = Addressables.LoadSceneAsync( 
					sceneContext.SceneName, LoadSceneMode.Additive, true, 100);
				s_ModuleHandle.Value.Completed += (handle) =>
				{
					if( handle.Status != AsyncOperationStatus.Succeeded)
					{
						s_ModuleHandle = null;
						StartCoroutine( Resume( sceneContext));
					}
				};
			}
		}
		IEnumerator Resume( SceneContext sceneContext)
		{
			IEnumerator it;
			
			m_RequestContext = null;
			
			if( m_SceneContext.IsModuleState( ModuleState.Opened) == false)
			{
				it = OnOpen();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_SceneContext.AddModuleState( ModuleState.Opened);
			}
			if( m_SceneContext.IsModuleState( ModuleState.Started) == false)
			{
				it = OnResume( sceneContext);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_SceneContext.AddModuleState( ModuleState.Started);
			}
			FocusIn();
		}
		IEnumerator Release()
		{
			++s_UnloadSceneCount;
			QuitModalModules();
			ModuleDaemon.Instance?.PopOverlayCamera( m_BaseCamera);
		#if UNITY_EDITOR
			float startSeconds = Time.realtimeSinceStartup;
		#endif
			if( OnInactivate() != false)
			{
				Inactivate();
			}
			IEnumerator it = OnPreUnload();
			
			while( it.MoveNext() != false)
			{
			#if UNITY_EDITOR
				if( Time.realtimeSinceStartup - startSeconds > 2.0f)
				{
					Debug.LogError( $"The previous module has not been terminated. {GetType()}");
				}
			#endif
				yield return null;
			}
			if( m_SceneContext != null)
			{
				while( m_SceneContext.IsLoading() != false)
				{
					yield return null;
				}
				m_SceneContext?.Releases();
			}
			if( m_ModuleHandle.HasValue != false)
			{
				Addressables.UnloadSceneAsync( m_ModuleHandle.Value, true);
			}
			else
			{
				SceneManager.UnloadSceneAsync( gameObject.scene);
			}
			--s_UnloadSceneCount;
		}
		void Inactivate()
		{
			GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
			
			for( int i0 = 0; i0 < rootObjects.Length; ++i0)
			{
				if( rootObjects[ i0] != gameObject)
				{
					rootObjects[ i0].SetActive( false);
				}
			}
			foreach( Transform child in transform)
			{
				child.gameObject.SetActive( false);
			}
		}
		internal Camera[] PushOverlayCamera( Camera overlayCamera)
		{
			Camera[] stackCameras = null;
			
			if( overlayCamera != null && m_BaseCamera != null)
			{
				var cameraStack = m_BaseCamera.GetUniversalAdditionalCameraData().cameraStack;
				
				if( (cameraStack?.Contains( overlayCamera) ?? true) == false)
				{
					overlayCamera.depth = m_CameraDepthOffset++;
					var overlayData = overlayCamera.GetUniversalAdditionalCameraData();
					var overlayStack = overlayData.cameraStack;
					int overlayStackCount = overlayStack?.Count ?? 0;
					
					stackCameras = new Camera[ overlayStackCount + 1];
					
					if( overlayStackCount > 0)
					{
						for( int i0 = 0; i0 < overlayStackCount; ++i0)
						{
							Camera childCamera = overlayStack[ i0];
							childCamera.depth = m_CameraDepthOffset++;
							
							if( cameraStack.Contains( childCamera) == false)
							{
								cameraStack.Add( childCamera);
							}
							stackCameras[ i0] = childCamera;
						}
						overlayStack.Clear();
					}
					if( overlayData.renderType != CameraRenderType.Overlay)
					{
						overlayData.renderType = CameraRenderType.Overlay;
					}
					stackCameras[ ^1] = overlayCamera;
					cameraStack.Add( overlayCamera);
					cameraStack.Sort( (a, b) => a.depth.CompareTo( b.depth));
				}
			}
			return stackCameras;
		}
		internal void PopOverlayCamera( params Camera[] cameras)
		{
			if( m_BaseCamera != null)
			{
				var cameraStack = m_BaseCamera.GetUniversalAdditionalCameraData().cameraStack;
				
				if( (cameraStack?.Count ?? 0) > 0)
				{
					for( int i0 = 0; i0 < cameras.Length; ++i0)
					{
						Camera camera = cameras[ i0];
						
						if( cameraStack.Contains( camera) != false)
						{
							if( m_CameraDepthOffset > camera.depth)
							{
								m_CameraDepthOffset = camera.depth;
							}
							cameraStack.Remove( camera);
						}
					}
				}
			}
		}
		protected private static SceneHandle s_CurrentHandle;
		protected private static AsyncOperationHandle<SceneInstance>? s_ModuleHandle;
		static int s_UnloadSceneCount;
		
		[System.NonSerialized]
		SceneContext m_SceneContext;
		[System.NonSerialized]
		SceneContext m_RequestContext;
		[System.NonSerialized]
		AsyncOperationHandle? m_ModuleHandle;
	}
}