
using UnityEngine;
using System.Collections;

namespace Knit.Framework
{
	public abstract partial class ModalHandle : ModuleHandle
	{
		internal void InitializeHandle( ModalContext modalContext, SceneHandle sceneHandle, ModuleHandle parentHandle, OnModalClosed onModalClosed)
		{
			m_ModalContext = modalContext;
			m_OnModalClosed = onModalClosed;
			m_SceneHandle = sceneHandle;
			
			if( m_BaseCamera != null)
			{
				m_StackCameras = m_SceneHandle.PushOverlayCamera( m_BaseCamera);
			}
		}
		protected virtual void Awake()
		{
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
			SetModuleContext( m_ModalContext = OnModalContext( m_ModalContext), this);
			
			if( m_ModalContext == null)
			{
				it = Release();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				yield break;
			}
			if( m_ModalContext.IsModuleState( ModuleState.PreLoaded) == false)
			{
				it = m_ModalContext.OnPreLoad();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_ModalContext.AddModuleState( ModuleState.PreLoaded);
			}
			m_ModalContext.AddModuleState( ModuleState.Instanced);
			
			if( m_ModalContext.IsModuleState( ModuleState.ContextPrepare) == false)
			{
				it = m_ModalContext.OnPrepare( () => bSuccess = false);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_ModalContext.AddModuleState( ModuleState.ContextPrepare);
			}
			if( m_ModalContext.IsModuleState( ModuleState.ModulePrepare) == false)
			{
				it = OnPrepare( () => bSuccess = false);
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_ModalContext.AddModuleState( ModuleState.ModulePrepare);
			}
			if( m_ModalContext.IsModuleState( ModuleState.Loaded) == false)
			{
				while( m_ModalContext.IsLoading() != false)
				{
					yield return null;
				}
				if( m_ModalContext.IsLoadFailed() != false)
				{
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_ModalContext.AddModuleState( ModuleState.Loaded);
			}
			if( m_ModalContext.IsModuleState( ModuleState.Setupd) == false)
			{
				it = OnSetup( () => bSuccess = false );
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				if( bSuccess == false)
				{
					it = Release();
					
					while( it.MoveNext() != false)
					{
						yield return null;
					}
					yield break;
				}
				m_ModalContext.AddModuleState( ModuleState.Setupd);
			}
			if( m_ModalContext.IsModuleState( ModuleState.Opened) == false)
			{
				it = OnOpen();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_ModalContext.AddModuleState( ModuleState.Opened);
			}
			if( m_ModalContext.IsModuleState( ModuleState.Started) == false)
			{
				it = OnStart();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
				m_ModalContext.AddModuleState( ModuleState.Started);
			}
			FocusIn();
		}
		IEnumerator Stop()
		{
			IEnumerator it;
			
			if( m_ModalContext.IsModuleState( ModuleState.Focus) != false)
			{
				m_ModalContext.RemoveModuleState( ModuleState.Focus);
				m_OnModuleSateFocus?.Invoke( false);
				OnFocusOut();
			}
			if( m_ModalContext.IsModuleState( ModuleState.Started) != false)
			{
				m_ModalContext.RemoveModuleState( ModuleState.Started);
				it = OnStop();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
			}
			if( m_ModalContext.IsModuleState( ModuleState.Opened) != false)
			{
				m_ModalContext.RemoveModuleState( ModuleState.Opened);
				it = OnClose();
				
				while( it.MoveNext() != false)
				{
					yield return null;
				}
			}
			if( m_ModalContext != null)
			{
				while( m_ModalContext.IsLoading() != false)
				{
					yield return null;
				}
			}
			if( m_StackCameras != null)
			{
				m_SceneHandle.PopOverlayCamera( m_StackCameras);
			}
			m_ModalContext?.Releases();
			m_OnModalClosed?.Invoke( m_ModalContext);
			Destroy( gameObject);
		}
		internal void Quit()
		{
			QuitModalModules();
			
			if( m_ModalContext.IsModuleState( ModuleState.Focus) != false)
			{
				m_ModalContext.RemoveModuleState( ModuleState.Focus);
				m_OnModuleSateFocus?.Invoke( false);
				OnFocusOut();
			}
			if( m_StackCameras != null)
			{
				m_SceneHandle.PopOverlayCamera( m_StackCameras);
			}
			m_ModalContext?.Releases();
			Destroy( gameObject);
		}
		IEnumerator Release()
		{
			yield break;
		}
		[System.NonSerialized]
		ModalContext m_ModalContext;
		[System.NonSerialized]
		OnModalClosed m_OnModalClosed;
		[System.NonSerialized]
		Camera[] m_StackCameras;
	}
}