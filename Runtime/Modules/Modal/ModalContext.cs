
namespace Knit.Framework
{
	public abstract partial class ModalContext : ModuleContext
	{
		protected ModalContext( string assetName) : base()
		{
			m_AssetName = assetName;
		}
		public void SetAssetKey( object assetKey)
		{
			m_AssetKey = assetKey;
		}
		public string AssetName
		{
			get{ return m_AssetName;}
		}
		public object AssetKey
		{
			get
			{
				if( m_AssetKey != null)
				{
					return m_AssetKey;
				}
				return m_AssetName;
			}
		}
        readonly string m_AssetName;
		object m_AssetKey;
	}
}