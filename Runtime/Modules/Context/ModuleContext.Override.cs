
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;

namespace Knit.Framework
{
	public abstract partial class ModuleContext : IModuleContext
	{
		protected internal virtual IEnumerator OnPreLoad()
		{
			yield break;
		}
		protected internal virtual IEnumerator OnPrepare( System.Action onFailure)
		{
			yield break;
		}
		
	}
}
