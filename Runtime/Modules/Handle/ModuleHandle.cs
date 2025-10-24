
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Knit.Framework
{
	internal delegate void OnModuleSateFocus( bool enable);
	
	public abstract partial class ModuleHandle : MonoBehaviour
	{
		private protected void SetModuleContext( ModuleContext moduleContext, ModuleHandle moduleHandle)
		{
			m_ModuleContext = moduleContext;
			
			if( moduleHandle is SceneHandle sceneHandle)
			{
				m_SceneHandle = sceneHandle;
			}
		}
		bool InstantiateModalModuleInternal( ModalHandle modalPrefab, ModalContext modalContext, OnModalClosed onModalClosed)
		{
			ModalHandle modalHandle = Instantiate( modalPrefab, transform, false);
			if( modalHandle != null)
			{
				modalHandle.name = modalPrefab.name;
				m_Modals[ modalContext] = modalHandle;
				modalHandle.InitializeHandle( modalContext, m_SceneHandle, this, onModalClosed);
				return true;
			}
			ModalModuleClosed( modalContext);
			return false;
		}
		void ModalModuleClosed( ModalContext modalContext)
		{
			m_Modals.Remove( modalContext);
			OnModalModuleClosed( modalContext);
			FocusIn();
		}
		private protected void QuitModalModules()
		{
			foreach( var modal in m_Modals)
			{
				modal.Value?.Quit();
			}
		}
		private protected void FocusIn()
		{
			if( m_Modals.Count == 0
			&&	m_ModuleContext.IsModuleState( ModuleState.FocusableMask, ModuleState.Focusable) != false)
			{
				m_ModuleContext.AddModuleState( ModuleState.Focus);
				m_OnModuleSateFocus?.Invoke( true);
				OnFocusIn();
			}
		}
		protected virtual void OnFocusSelectableObject()
		{
			var selectable = Selectable.OrderSelectable();
			
			if( selectable != null)
			{
				EventSystem.current?.SetSelectedGameObject( selectable.gameObject);
			}
		}
		internal event OnModuleSateFocus OnFocus
		{
			add
			{
				m_OnModuleSateFocus += value;
				
				if( (m_ModuleContext?.IsModuleState( ModuleState.Focus) ?? false) != false)
				{
					value.Invoke( true);
				}
			}
			remove{ m_OnModuleSateFocus -= value; }
		}
		[SerializeField]
		internal ModuleSetting m_ModuleSetting;
		[SerializeField]
		internal Camera m_BaseCamera;
		
		[System.NonSerialized]
		ModuleContext m_ModuleContext;
		[System.NonSerialized]
		private protected SceneHandle m_SceneHandle;
		[System.NonSerialized]
		private protected float m_CameraDepthOffset;
		[System.NonSerialized]
		private protected OnModuleSateFocus m_OnModuleSateFocus;
		[System.NonSerialized]
        readonly Dictionary<ModalContext, ModalHandle> m_Modals = new();
	}
}