
using UnityEngine;

namespace Knit.Framework
{
	public abstract partial class ModuleHandle : MonoBehaviour
	{
		protected virtual void OnFocusIn()
		{
		#if ENABLE_INPUT_SYSTEM
			ModuleDaemon.Instance.OnInputSubmit += OnInputSubmit;
			ModuleDaemon.Instance.OnInputCancel += OnInputCancel;
			ModuleDaemon.Instance.OnFocusSelectableObject += OnFocusSelectableObject;
		#endif
		}
		protected virtual void OnFocusOut()
		{
		#if ENABLE_INPUT_SYSTEM
			ModuleDaemon.Instance.OnInputSubmit -= OnInputSubmit;
			ModuleDaemon.Instance.OnInputCancel -= OnInputCancel;
			ModuleDaemon.Instance.OnFocusSelectableObject -= OnFocusSelectableObject;
		#endif
		}
		protected virtual void OnInputSubmit()
		{
		}
		protected virtual void OnInputCancel()
		{
		}
		protected virtual void OnModalModuleOpen( ModuleContext moduleContext)
		{
		}
		protected virtual void OnModalModuleClosed( ModuleContext moduleContext)
		{
		}
	}
}