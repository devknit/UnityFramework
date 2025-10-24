
using UnityEngine;

namespace Knit.Framework.Sample
{
	public abstract class ModalModuleT<TContext>
		: ModalHandleT<TContext, ModuleSetting, ModuleDaemon> where TContext : ModalContext
	{
	}
}