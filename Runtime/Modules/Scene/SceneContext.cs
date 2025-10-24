
namespace Knit.Framework
{
	public abstract class SceneContext : ModuleContext
	{
		protected SceneContext( string sceneName) : base()
		{
			m_SceneName = sceneName;
		}
		public string SceneName
		{
			get{ return m_SceneName; }
		}
		public SceneContext PrevContext
		{
			get{ return m_PrevContext; }
			internal set{ m_PrevContext = value; }
		}
		public virtual bool IsRestorableContext
		{
			get{ return true; }
			set{}
		}
		[System.NonSerialized]
		string m_SceneName;
		[System.NonSerialized]
		SceneContext m_PrevContext;
	}
}