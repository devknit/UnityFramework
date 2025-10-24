
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace Knit.Framework
{
	public abstract partial class ModuleContext : IModuleContext
	{
		internal bool CanBoot()
		{
			return (m_ModuleState & ModuleState.ActiveMask) == ModuleState.None;
		}
		internal void AddModuleState( ModuleState moduleState)
		{
			m_ModuleState |= moduleState;
		}
		internal void RemoveModuleState( ModuleState moduleState)
		{
			m_ModuleState &= ~moduleState;
		}
		internal bool IsModuleState( ModuleState moduleState)
		{
			return (m_ModuleState & moduleState) == moduleState;
		}
		internal bool IsModuleState( ModuleState stateMask, ModuleState stateValue)
		{
			return (m_ModuleState & stateMask) == stateValue;
		}
		protected internal virtual bool Releases()
		{
			if( IsLoading() != false)
			{
				return false;
			}
			if( m_AssetLoadCaches.Count > 0)
			{
				foreach( var value in m_AssetLoadCaches.Values)
				{
					if( value.handle.IsValid() != false)
					{
						Addressables.Release( value.handle);
					}
				}
				m_AssetLoadCaches.Clear();
				m_AssetLoadSucceededCount = 0;
				m_AssetLoadFailedCount = 0;
			}
			if( m_AmbientLoadCaches.Count > 0)
			{
				foreach( var sceneName in m_AmbientLoadCaches)
				{
					AmbientModule.Release( sceneName);
				}
				m_AmbientLoadCaches.Clear();
			}
			m_ModuleState = ModuleState.Finished;
			return true;
		}
		AsyncOperationHandle<TObject>? InternalLoadAssetAsync<TObject>( int id, object key)
		{
			if( m_AssetLoadCaches.TryGetValue( id, out LoadAssetCache cache) != false)
			{
				return cache.handle.Convert<TObject>();
			}
			else
			{
				AsyncOperationHandle<TObject> handle = Addressables.LoadAssetAsync<TObject>( key);
				if( handle.IsValid() != false)
				{
					var instance = new LoadAssetCache
					{
						key = key, 
						handle = handle
					};
					handle.Completed += (op) =>
					{
						instance.result = op.Status;
						
						switch( instance.result)
						{
							case AsyncOperationStatus.Succeeded: ++m_AssetLoadSucceededCount; break;
							case AsyncOperationStatus.Failed: ++m_AssetLoadFailedCount; break;
						}
					};
					m_AssetLoadCaches.Add( id, instance);
					return handle;
				}
			#if UNITY_EDITOR
				else{ Debug.LogError( $"LoadAssetAsync( \"{id}\", {key})\n読み込みの申請に失敗しました。\n"); }
			#endif
			}
			return null;
		}
		sealed class LoadAssetCache
		{
			internal object key;
			internal AsyncOperationHandle handle;
			internal AsyncOperationStatus result;
		}
        readonly Dictionary<int, LoadAssetCache> m_AssetLoadCaches = new();
        readonly HashSet<string> m_AmbientLoadCaches = new();
		int m_AssetLoadSucceededCount;
		int m_AssetLoadFailedCount;
		ModuleState m_ModuleState;
	}
}
