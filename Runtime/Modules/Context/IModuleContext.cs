
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Knit.Framework
{
	public interface IModuleContext
	{
		bool LoadAmbientSceneAsync( string sceneName);
		bool ReleaseAmbientScene( string sceneName);
		AsyncOperationHandle<TObject>? LoadAssetAsync<TObject>( int id, AssetReference key);
		AsyncOperationHandle<TObject>? LoadAssetAsync<TObject>( int id, string key);
		bool TryGetLoadAssetHandle( int id, out AsyncOperationHandle? handle);
		bool ReleaseAsset( int id);
	}
}
