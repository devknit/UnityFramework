
using System.Collections;

namespace Knit.Framework
{
	public abstract partial class ModalHandle : ModuleHandle
	{
		protected private abstract ModalContext OnModalContext( ModalContext modalContext);
		
		protected virtual IEnumerator OnPrepare( System.Action onFailure)
		{
			yield break;
		}
		protected virtual IEnumerator OnSetup( System.Action onFailure)
		{
			yield break;
		}
		protected virtual IEnumerator OnOpen()
		{
			yield break;
		}
		protected virtual IEnumerator OnStart()
		{
			yield break;
		}
		protected virtual IEnumerator OnStop()
		{
			yield break;
		}
		protected virtual IEnumerator OnClose()
		{
			yield break;
		}
		protected virtual IEnumerator OnFinalize()
		{
			yield break;
		}
		protected virtual IEnumerator OnPreUnload()
		{
			yield break;
		}
	}
}