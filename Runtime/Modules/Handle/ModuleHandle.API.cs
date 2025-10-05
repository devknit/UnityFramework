
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

namespace Knit.Framework
{
	public delegate void OnModalClosed( ModalContext modalContext);
	
	public abstract partial class ModuleHandle : MonoBehaviour
	{
		public bool Interactable
		{
			get{ return (m_ModuleContext?.IsModuleState( ModuleState.Focus) ?? false) != false; }
		}
		public bool InstantiateModalModule( ModalContext modalContext)
		{
			if( (modalContext?.CanBoot() ?? false) == false || m_Modals.ContainsKey( modalContext) != false)
			{
				return false;
			}
			if( m_ModuleContext.IsModuleState( ModuleState.Focus) != false)
			{
				m_ModuleContext.RemoveModuleState( ModuleState.Focus);
				m_OnModuleSateFocus?.Invoke( false);
				OnFocusOut();
			}
			m_Modals.Add( modalContext, null);
			OnModalModuleOpen( modalContext);
			
			if( m_ModuleSetting.TryGetModalModulePrefab( modalContext.AssetName, out ModalHandle modalPrefab) != false)
			{
				if( modalPrefab != null)
				{
					StartCoroutine( InstantiateModalModule( modalContext, modalPrefab));
					return true;
				}
			}
			AsyncOperationHandle<GameObject> handle = 
				Addressables.LoadAssetAsync<GameObject>( modalContext.AssetKey);
			if( handle.IsValid() != false)
			{
				StartCoroutine( InstantiateModalModule( modalContext, handle));
				return true;
			}
			return false;
		}
		IEnumerator InstantiateModalModule( ModalContext modalContext, ModalHandle modalPrefab)
		{
			IEnumerator it = modalContext.OnPreLoad();
			
			while( it.MoveNext() != false)
			{
				yield return null;
			}
			InstantiateModalModuleInternal( modalPrefab, modalContext, ModalModuleClosed);
		}
		IEnumerator InstantiateModalModule( ModalContext modalContext, AsyncOperationHandle<GameObject> handle)
		{
			IEnumerator it = modalContext.OnPreLoad();
			ModalHandle modalPrefab = null;
			
			while( it.MoveNext() != false)
			{
				yield return null;
			}
			yield return handle;
			
			if( handle.Status == AsyncOperationStatus.Succeeded)
			{
				modalPrefab = handle.Result.GetComponent<ModalHandle>();
			}
			if( modalPrefab != null)
			{
				InstantiateModalModuleInternal( modalPrefab, modalContext, (modalContext) =>
				{
					ModalModuleClosed( modalContext);
					Addressables.Release( handle);
				});
			}
			else
			{
				ModalModuleClosed( modalContext);
				Addressables.Release( handle);
			}
		}
	}
}