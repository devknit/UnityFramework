
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Knit.Framework
{
	enum AmbientActivate
	{
		Error,
		Idle,
		Forcus,
	}
	enum AmbientRelease
	{
		Error,
		Keep,
		Unload,
		ForcusUnload,
	}
	sealed class AmbientCahce
	{
		internal AmbientCahce( AsyncOperationHandle<SceneInstance> handle, int priority)
		{
			m_Handle = handle;
			m_Priority= priority;
			m_Reference = 1;
		}
		internal AmbientCahce( AmbientModule ambientHandle, int priority)
		{
			m_Module = ambientHandle;
			m_Priority= priority;
			m_Reference = 0;
		}
		internal void SetAmbientHandle( AmbientModule ambientHandle)
		{
			m_Module = ambientHandle;
			SetActive( false);
		}
		internal AmbientActivate Activated()
		{
			if( m_Module != null)
			{
				if( m_Priority == 0)
				{
					return AmbientActivate.Forcus;
				}
				return AmbientActivate.Idle;
			}
			return AmbientActivate.Error;
		}
		internal AmbientRelease Release()
		{
			if( IsLoading() == false)
			{
				if( m_Reference > 0)
				{
					--m_Reference;
				}
				if( m_Reference <= 0)
				{
					SetActive( false);
					
					if( m_Handle.IsValid() != false)
					{
						Addressables.UnloadSceneAsync( m_Handle);
					}
					else if( m_Module != null)
					{
						SceneManager.UnloadSceneAsync( m_Module.gameObject.scene);
					}
					return (m_Priority == 0)? AmbientRelease.ForcusUnload : AmbientRelease.Unload;
				}
				return AmbientRelease.Keep;
			}
			return AmbientRelease.Error;
		}
		internal bool IsLoading()
		{
			if( m_Handle.IsValid() != false)
			{
				if( m_Handle.IsDone == false || m_Handle.Result.Scene.isLoaded == false)
				{
					return true;
				}
			}
			if( m_Module != null)
			{
				if( m_Module.gameObject.scene.isLoaded == false)
				{
					return true;
				}
			}
			return false;
		}
		internal bool Forcus( bool enable)
		{
			if( enable != false)
			{
				if( m_Actived != false)
				{
					return false;
				}
				m_Priority = 0;
			}
			else
			{
				++m_Priority;
			}
			if( m_Module != null)
			{
				if( m_Priority > 0)
				{
					SetActive( false);
				}
				else
				{
					SetActive( true);
					SceneManager.SetActiveScene( m_Module.gameObject.scene);
					return true;
				}
			}
			return false;
		}
		internal int GetForcusPriority()
		{
			return m_Priority;
		}
		internal void Reference()
		{
			++m_Reference;
		}
		void SetActive( bool enable)
		{
			if( m_Module != null)
			{
				GameObject[] gameObjects = m_Module.gameObject.scene.GetRootGameObjects();
				
				for( int i0 = 0; i0 < gameObjects.Length; ++i0)
				{
					gameObjects[ i0].SetActive( enable);
				}
			}
			m_Actived = enable;
		}
		internal int Priority
		{
			get{ return m_Priority; }
		}
		internal AmbientModule Module
		{
			get{ return m_Module; }
		}
		bool m_Actived;
		int m_Reference;
		int m_Priority = -1;
		AmbientModule m_Module;
		AsyncOperationHandle<SceneInstance> m_Handle;
	}
}