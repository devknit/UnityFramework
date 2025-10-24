
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Knit.Framework
{
	[Serializable]
	sealed class SelectableAudioUnity : SelectableAudioT<SelectableStateAudioUnity, SelectableAudioUnityContext>
	{
		protected override SelectableAudioContext Play( SelectionState state)
		{
			if( TryGetStateAudio( state, out SelectableStateAudioUnity audio) != false)
			{
				return new SelectableAudioUnityContext(
					AudioSourcePool.Instance.Play( audio.Clip, m_MixerGroup, 1.0f, false));
			}
			return null;
		}
		protected override void Stop( SelectableAudioContext context)
		{
			if( context is SelectableAudioUnityContext clipContext)
			{
				clipContext.Stop();
			}
		}
		[SerializeField]
		AudioMixerGroup m_MixerGroup;
	}
	sealed class SelectableAudioUnityContext : SelectableAudioContext
	{
		internal SelectableAudioUnityContext( AudioSource source)
		{
			m_Source = source;
		}
		internal void Stop()
		{
			m_Source?.Stop();
		}
		protected override bool IsPlaying()
		{
			return m_Source?.isPlaying ?? false;
		}
        readonly AudioSource m_Source;
	}
	[Serializable]
	sealed class SelectableStateAudioUnity : SelectableStateAudio
	{
		protected override bool IsValid()
		{
			return m_AudioClip != null;
		}
		internal AudioClip Clip
		{
			get{ return m_AudioClip; }
		}
		[SerializeField]
		AudioClip m_AudioClip;
	}
}
