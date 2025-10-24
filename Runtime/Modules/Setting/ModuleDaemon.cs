
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Knit.Framework
{
	public class ModuleDaemon : MonoBehaviour
	{
		public static ModuleDaemon Instance
		{
			get;
			private set;
		}
		protected internal virtual bool IsInitialized
		{
			get{ return true; }
		}
		protected internal EventSystem EventSystem
		{
			get{ return m_EventSystem; }
		}
		protected internal BaseInputModule InputModule
		{
			get{ return m_InputModule; }
		}
		protected internal AudioListener AudioListener
		{
			get{ return m_AudioListener; }
		}
	#if ENABLE_INPUT_SYSTEM
		protected internal event System.Action OnInputSubmit
		{
			add{ m_OnInputSubmit += value; }
			remove{ m_OnInputSubmit -= value; }
		}
		protected internal event System.Action OnInputCancel
		{
			add{ m_OnInputCancel += value; }
			remove{ m_OnInputCancel -= value; }
		}
		protected internal event System.Action OnFocusSelectableObject
		{
			add{ m_OnFocusSelectableObject += value; }
			remove{ m_OnFocusSelectableObject -= value; }
		}
	#endif
		protected virtual void OnAwake()
		{
		}
		internal void PushOverlayCamera( Camera baseCamera)
		{
			if( (m_OverlayCameras?.Length ?? 0) > 0 && baseCamera != null)
			{
				var cameraStack = baseCamera.GetUniversalAdditionalCameraData().cameraStack;
				
				if( cameraStack != null)
				{
					for( int i0 = 0; i0 < m_OverlayCameras.Length; ++i0)
					{
						if( cameraStack.Contains( m_OverlayCameras[ i0]) == false)
						{
							cameraStack.Add( m_OverlayCameras[ i0]);
						}
					}
					cameraStack.Sort( (a, b) => a.depth.CompareTo( b.depth));
				}
			}
		}
		internal void PopOverlayCamera( Camera baseCamera)
		{
			if( (m_OverlayCameras?.Length ?? 0) > 0 && baseCamera != null)
			{
				var cameraStack = baseCamera.GetUniversalAdditionalCameraData().cameraStack;
				
				if( cameraStack != null)
				{
					for( int i0 = 0; i0 < m_OverlayCameras.Length; ++i0)
					{
						if( cameraStack.Contains( m_OverlayCameras[ i0]) != false)
						{
							cameraStack.Remove( m_OverlayCameras[ i0]);
						}
					}
				}
			}
		}
		void Awake()
		{
			if( Instance != null)
			{
				Destroy( gameObject);
			}
			else
			{
			#if ENABLE_INPUT_SYSTEM
				if( m_InputModule != null)
				{
					var inputModule = m_InputModule.GetComponent<InputSystemUIInputModule>();
					
					if( inputModule != null)
					{
						if( inputModule.submit != null)
						{
							inputModule.submit.action.started += context =>
							{
								m_OnInputSubmit?.Invoke();
							};
						}
						if( inputModule.cancel != null)
						{
							inputModule.cancel.action.started += context =>
							{
								m_OnInputCancel?.Invoke();
							};
						}
						if( inputModule.move != null)
						{
							inputModule.move.action.performed += context =>
							{
								if( EventSystem.current.currentSelectedGameObject == null)
								{
									StartCoroutine( OnOnFocusSelectableObject());
								}
							};
						}
					}
				}
			#endif
				if( m_OverlayCamera != null)
				{
					var cameraData = m_OverlayCamera.GetUniversalAdditionalCameraData();
					var cameraStack = cameraData.cameraStack;
					int cameraStackCount = cameraStack?.Count ?? 0;
					float depth = 100;
					
					m_OverlayCameras = new Camera[ cameraStackCount + 1];
					
					if( cameraData.renderType != CameraRenderType.Overlay)
					{
						cameraData.renderType = CameraRenderType.Overlay;
					}
					if( cameraStackCount > 0)
					{
						for( int i0 = cameraStackCount - 1; i0 >= 0; --i0)
						{
							cameraStack[ i0].depth = depth--;
							m_OverlayCameras[ i0] = cameraStack[ i0];
						}
					}
					m_OverlayCamera.depth = depth;
					m_OverlayCameras[ ^1] = m_OverlayCamera;
				}
				Instance = this;
				DontDestroyOnLoad( gameObject);
				OnAwake();
			}
		}
	#if ENABLE_INPUT_SYSTEM
		IEnumerator OnOnFocusSelectableObject()
		{
			yield return new WaitForEndOfFrame();
			
			if( EventSystem.current.currentSelectedGameObject == null)
			{
				m_OnFocusSelectableObject?.Invoke();
			}
		}
	#endif
		[SerializeField]
		EventSystem m_EventSystem;
		[SerializeField]
		BaseInputModule m_InputModule;
		[SerializeField]
		AudioListener m_AudioListener;
		[SerializeField]
		Camera m_OverlayCamera;
		[System.NonSerialized]
		Camera[] m_OverlayCameras;
	#if ENABLE_INPUT_SYSTEM
		System.Action m_OnInputSubmit;
		System.Action m_OnInputCancel;
		System.Action m_OnFocusSelectableObject;
	#endif
	}
}