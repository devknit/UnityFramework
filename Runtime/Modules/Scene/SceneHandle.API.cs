
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Knit.Framework
{
	public abstract partial class SceneHandle : ModuleHandle
	{
		public Camera BaseCaemra
		{
			get{ return m_BaseCamera; }
		}
		public static AsyncOperationHandle<SceneInstance> Boot( object bootSceneKey)
		{
			if( s_ModuleHandle.HasValue != false)
			{
				throw new System.InvalidOperationException( "Already received your request.");
			}
			AsyncOperationHandle<SceneInstance> handle = 
				Addressables.LoadSceneAsync( bootSceneKey, LoadSceneMode.Single, false, 100);
			s_ModuleHandle = handle;
			return handle;
		}
		protected internal bool RequestLoadScene( SceneContext sceneContext)
		{
			if( sceneContext == null)
			{
				return false;
			}
			SceneContext targetContext = m_SceneContext;
			
			while( (targetContext?.IsRestorableContext ?? true) == false)
			{
				targetContext = targetContext.PrevContext;
			}
			sceneContext.PrevContext = targetContext;
			
			StartCoroutine( Stop( sceneContext, false));
			return true;
		}
		protected internal bool RequestReloadScene( SceneContext sceneContext)
		{
			if( sceneContext == null)
			{
				return false;
			}
			sceneContext.PrevContext = m_SceneContext.PrevContext;
			
			StartCoroutine( Stop( sceneContext, true));
			return true;
		}
		protected internal bool RequestBackScene( System.Func<SceneContext> onMissing)
		{
			SceneContext sceneContext = m_SceneContext.PrevContext;
			
			sceneContext ??= onMissing?.Invoke();
			
			if( sceneContext == null)
			{
				return false;
			}
			StartCoroutine( Stop( sceneContext, false));
			return true;
		}
	}
}