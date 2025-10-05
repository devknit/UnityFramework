
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Knit.Framework.Sample
{
	sealed class YellowModule : ModalModuleT<YellowContext>
	{
		protected override YellowContext CreateDefaultContext()
		{
			return new YellowContext( null);
		}
		protected override IEnumerator OnPrepare( System.Action onFailure)
		{
			m_CanvasGroup.alpha = 0;
			Debug.Log( $"<color=yellow>{GetType().Name}: OnPrepare\n");
			yield break;
		}
		protected override IEnumerator OnSetup( System.Action onFailure)
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnSetup\n");
			yield break;
		}
		protected override IEnumerator OnOpen()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnOpen\n");
			const float duration = 0.4f;
			float elapsedTime = 0;
			
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				m_CanvasGroup.alpha = Mathf.Clamp01( elapsedTime / duration);
				yield return null;
			}
			m_CanvasGroup.alpha = 1.0f;
			yield break;
		}
		protected override IEnumerator OnStart()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnStart\n");
			yield break;
		}
		protected override IEnumerator OnClose()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnClose\n");
			const float duration = 0.2f;
			float elapsedTime = 0;
			
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				m_CanvasGroup.alpha = 1.0f - Mathf.Clamp01( elapsedTime / duration);
				yield return null;
			}
			m_CanvasGroup.alpha = 0.0f;
			yield break;
		}
		protected override IEnumerator OnStop()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnStop\n");
			yield break;
		}
		protected override IEnumerator OnPreUnload()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnPreUnload\n");
			yield break;
		}
		protected override void OnFocusIn()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnFocusIn\n");
			EventSystem.current?.SetSelectedGameObject( GetComponentInChildren<Selectable>()?.gameObject);
		}
		protected override void OnFocusOut()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnFocusOut\n");
		}
		protected override void OnModalModuleOpen( ModuleContext moduleContext)
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnModalModuleOpen - {moduleContext.GetType().Name}\n");
		}
		protected override void OnModalModuleClosed( ModuleContext moduleContext)
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnModalModuleClosed - {moduleContext.GetType().Name}\n");
		}
		public void OnYes( Selectable selectable)
		{
			Exit();
			Context.Callback?.Invoke( true);
		}
		public void OnNo( Selectable selectable)
		{
			Exit();
			Context.Callback?.Invoke( false);
		}
		[SerializeField]
		CanvasGroup m_CanvasGroup;
	}
}