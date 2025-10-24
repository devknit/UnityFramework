
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

namespace Knit.Framework.Sample
{
	sealed class GreenModule : SceneModuleT<GreenContext>
	{
		protected override GreenContext CreateDefaultContext()
		{
			return new GreenContext();
		}
		protected override IEnumerator OnPrepare( System.Action onFailure)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnPrepare\n");
			yield break;
		}
		protected override IEnumerator OnSetup( System.Action onFailure)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnSetup\n");
			string activeScene = SceneManager.GetActiveScene().name;
			m_Toggles.FirstOrDefault( x => x.name == activeScene)?.Set( true);
			m_Slider.Value = Setting.BackgroundVolume;
			yield break;
		}
		protected override IEnumerator OnStart()
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnStart\n");
			yield break;
		}
		protected override IEnumerator OnStop( Framework.SceneContext requestSceneContext, System.Action onFailure)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnStop\n");
			yield break;
		}
		protected override IEnumerator OnPreUnload()
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnPreUnload\n");
			yield break;
		}
		protected override IEnumerator OnResume( Framework.SceneContext finishedSceneContext)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnResume\n");
			yield break;
		}
		protected override void OnFocusIn()
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnFocusIn\n");
			EventSystem.current?.SetSelectedGameObject( 
				m_LastSelectable?.gameObject ?? GetComponentInChildren<Selectable>()?.gameObject);
		}
		protected override void OnFocusOut()
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnFocusOut\n");
		}
		protected override void OnModalModuleOpen( ModuleContext moduleContext)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnModalModuleOpen - {moduleContext.GetType().Name}\n");
		}
		protected override void OnModalModuleClosed( ModuleContext moduleContext)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnModalModuleClosed - {moduleContext.GetType().Name}\n");
		}
		public void OnToggle( Selectable selectable, bool isOn)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnToggle( {selectable.name}, {isOn})\n");
			
			if( Context != null && isOn != false)
			{
				Context.LoadAmbientSceneAsync( selectable.name);
				AmbientModule.Forcus( selectable.name);
			}
		}
		public void OnSlider( Selectable selectable, float value)
		{
			Setting.BackgroundVolume = Mathf.Clamp01( value);
		}
		public void OnBack( Selectable selectable)
		{
			m_LastSelectable = selectable;
			RequestBackScene( null);
		}
		public void OnReload( Selectable selectable)
		{
			RequestReloadScene( new GreenContext());
		}
		public void OnNext( Selectable selectable)
		{
			m_LastSelectable = selectable;
			InstantiateModalModule( new YellowContext( (result) => 
			{
				if( result != false)
				{
					RequestLoadScene( new RedContext());
				}
			}));
		}
		[SerializeField]
		Slider m_Slider;
		[SerializeField]
		Toggle[] m_Toggles;
		Selectable m_LastSelectable;
	}
}