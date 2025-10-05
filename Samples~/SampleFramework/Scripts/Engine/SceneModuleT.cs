
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Knit.Framework.Sample
{
	public abstract class SceneModuleT<TContext>
		: SceneHandleT<TContext, ModuleSetting, ModuleDaemon> where TContext : SceneContext
	{
		protected sealed override IEnumerator OnOpen()
		{
			Debug.Log( $"{GetType().Name}: OnOpen\n");
			Daemon.StartCoroutine( Setting.PlayBackgroundMusic( m_BackgroundClip));
			IEnumerator it = Daemon.FadeOut( 0.4f);
			
			while( it.MoveNext() != false)
			{
				yield return null;
			}
		}
		protected sealed override IEnumerator OnClose()
		{
			Debug.Log( $"{GetType().Name}: OnClose\n");
			IEnumerator it = Daemon.FadeIn( 0.4f);
			
			while( it.MoveNext() != false)
			{
				yield return null;
			}
		}
		[SerializeField]
		AudioClip m_BackgroundClip;
	}
}