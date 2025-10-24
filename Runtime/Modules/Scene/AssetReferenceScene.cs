
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Knit.Framework
{
	[System.Serializable]
	public sealed class AssetReferenceScene : AssetReference
	{
		public AssetReferenceScene( string guid) : base( guid)
		{
		}
		public override bool ValidateAsset( Object obj)
		{
		#if UNITY_EDITOR
			return typeof( UnityEditor.SceneAsset).IsAssignableFrom( obj.GetType());
		#else
			return false;
		#endif
		}
		public override bool ValidateAsset( string path)
		{
		#if UNITY_EDITOR
			return typeof( UnityEditor.SceneAsset).IsAssignableFrom( 
				UnityEditor.AssetDatabase.GetMainAssetTypeAtPath( path));
		#else
			return false;
		#endif
		}
	#if UNITY_EDITOR
		public new UnityEditor.SceneAsset editorAsset => (UnityEditor.SceneAsset)base.editorAsset;
	#endif
	}
}