
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Knit.Framework
{
	public enum ModuleState
	{
		None = 0,
		PreLoaded = 1 << 0,
		Instanced = 1 << 1,
		ContextPrepare = 1 << 2,
		ModulePrepare = 1 << 3,
		Loaded = 1 << 4,
		Setupd = 1 << 5,
		Opened = 1 << 6,
		Started = 1 << 7,
		Focus = 1 << 8,
		Focusable = Instanced | ContextPrepare | ModulePrepare | Loaded | Setupd | Opened | Started,
		FocusableMask = Focusable | Focus,
		Finished = 1 << 9,
		ActiveMask = ~Finished,
	}
	public abstract partial class ModuleContext : IModuleContext
	{
		public bool IsFinished
		{
			get{ return IsModuleState( ModuleState.Finished); }
		}
		public bool LoadAmbientSceneAsync( string sceneName)
		{
			if( m_AmbientLoadCaches.Contains( sceneName) == false)
			{
				if( AmbientModule.LoadAsync( sceneName) != false)
				{
					m_AmbientLoadCaches.Add( sceneName);
					return true;
				}
			}
			return false;
		}
		public bool ReleaseAmbientScene( string sceneName)
		{
			if( m_AmbientLoadCaches.Contains( sceneName) != false)
			{
				if( AmbientModule.Release( sceneName) != false)
				{
					m_AmbientLoadCaches.Remove( sceneName);
				}
			}
			return false;
		}
		public AsyncOperationHandle<TObject>? LoadAssetAsync<TObject>( int id, AssetReference key)
		{
			if( key?.RuntimeKeyIsValid() != false)
			{
				return InternalLoadAssetAsync<TObject>( id, key);
			}
			return null;
		}
		public AsyncOperationHandle<TObject>? LoadAssetAsync<TObject>( int id, string key)
		{
			if( string.IsNullOrEmpty( key) == false)
			{
				return InternalLoadAssetAsync<TObject>( id, key);
			}
			return null;
		}
		public bool TryGetLoadAssetHandle( int id, out AsyncOperationHandle? handle)
		{
			if( (m_AssetLoadCaches?.TryGetValue( id, out LoadAssetCache cache) ?? false) != false)
			{
				handle = cache.handle;
				return true;
			}
			handle = null;
			return false;
		}
		public bool IsLoading()
		{
			return AmbientModule.IsLoading() != false
			||	(m_AssetLoadSucceededCount + m_AssetLoadFailedCount) < (m_AssetLoadCaches?.Count ?? 0);
		}
		public bool IsLoadFailed()
		{
			return m_AssetLoadFailedCount > 0;
		}
		public bool ReleaseAsset( int id)
		{
			if( m_AssetLoadCaches != null)
			{
				if( m_AssetLoadCaches.TryGetValue( id, out LoadAssetCache instance) != false)
				{
					if( instance.result != AsyncOperationStatus.None)
					{
						switch( instance.result)
						{
							case AsyncOperationStatus.Succeeded: --m_AssetLoadSucceededCount; break;
							case AsyncOperationStatus.Failed: --m_AssetLoadFailedCount; break;
						}
						if( instance.handle.IsValid() != false)
						{
							Addressables.Release( instance.handle);
						}
						m_AssetLoadCaches.Remove( id);
						return true;
					}
				}
			}
			return false;
		}
	}
}
