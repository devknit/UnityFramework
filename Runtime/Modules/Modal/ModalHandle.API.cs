
namespace Knit.Framework
{
	public abstract partial class ModalHandle : ModuleHandle
	{
		public void Exit()
		{
			StartCoroutine( Stop());
		}
		protected bool RequestLoadScene( SceneContext sceneContext)
		{
			return m_SceneHandle?.RequestLoadScene( sceneContext) ?? false;
		}
		protected bool RequestReloadScene( SceneContext sceneContext)
		{
			return m_SceneHandle?.RequestReloadScene( sceneContext) ?? false;
		}
		protected bool RequestBackScene( System.Func<SceneContext> onMissing)
		{
			return m_SceneHandle?.RequestBackScene( onMissing) ?? false;
		}
	}
}