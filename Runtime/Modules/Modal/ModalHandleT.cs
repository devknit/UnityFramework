
namespace Knit.Framework
{
	public abstract class ModalHandleT<TContext, TSetting, TDaemon> : ModalHandle
		where TContext : ModalContext
		where TSetting : ModuleSetting
		where TDaemon  : ModuleDaemon
	{
		protected abstract TContext CreateDefaultContext();
		
		protected private override ModalContext OnModalContext( ModalContext modalContext)
		{
			if( modalContext is TContext moduleContext)
			{
				m_Context = moduleContext;
			}
			m_Context ??= CreateDefaultContext();
			m_Setting = m_ModuleSetting as TSetting;
			m_Daemon = ModuleDaemon.Instance as TDaemon;
			return m_Context;
		}
		public TContext Context
		{
			get{ return m_Context; }
		}
		public TSetting Setting
		{
			get{ return m_Setting; }
		}
		public TDaemon Daemon
		{
			get{ return m_Daemon; }
		}
		[System.NonSerialized]
		TContext m_Context;
		[System.NonSerialized]
		TSetting m_Setting;
		[System.NonSerialized]
		TDaemon m_Daemon;
	}
}