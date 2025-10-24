
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;

namespace Knit.Framework.Sample
{
	public sealed class ModuleSetting : Framework.ModuleSetting
	{
		public float BackgroundVolume
		{
			get{ if( m_AudioMixer.GetFloat( m_BackgroundMixerGroup.name, out float volume) != false){ return DecibelToSquared( volume); } return 1.0f; }
			set{ m_AudioMixer.SetFloat( m_BackgroundMixerGroup.name, SquaredToDecibel( value)); }
		}
		internal IEnumerator PlayBackgroundMusic( AudioClip audioClip)
		{
			if( s_CurrentBackgroundClip != audioClip)
			{
				const float fadeDuration = 0.5f;
				
				if( s_CurrentBackgroundClip != null)
				{
					float elapsedTime = 0.0f;
					
					while( elapsedTime < fadeDuration)
					{
						elapsedTime += Time.deltaTime;
						
						if( elapsedTime >= fadeDuration)
						{
							break;
						}
						s_CurrentBackgroundSource.volume = 1.0f - Mathf.Clamp01( elapsedTime / fadeDuration);
						yield return null;
					}
					s_CurrentBackgroundSource.volume = 0.0f;
					s_CurrentBackgroundSource.Stop();
					s_CurrentBackgroundSource = null;
					s_CurrentBackgroundClip = null;
				}
				if( audioClip != null)
				{
					s_CurrentBackgroundSource = AudioSourcePool.Instance.Play( audioClip, m_BackgroundMixerGroup, 0.0f, true);
					s_CurrentBackgroundClip = audioClip;
					float elapsedTime = 0.0f;
					
					while( elapsedTime < fadeDuration)
					{
						elapsedTime += Time.deltaTime;
						
						if( elapsedTime >= fadeDuration)
						{
							break;
						}
						s_CurrentBackgroundSource.volume = Mathf.Clamp01( elapsedTime / fadeDuration);
						yield return null;
					}
					s_CurrentBackgroundSource.volume = 1.0f;
				}
			}
		}
		public static float SquaredToDecibel( float squaredValue)
		{
			squaredValue = Mathf.Sqrt( squaredValue);
			return (squaredValue * kMinimumDecibel) - kMinimumDecibel;
		}
		public static float DecibelToSquared( float decibel)
		{
			float value = (decibel + kMinimumDecibel) / kMinimumDecibel;
			return value * value;
		}
		public static float DecibelToLinear( float decibelVolume)
		{
			return Mathf.Pow( 10.0f, decibelVolume / 20.0f);
		}
		public static float LinearTODecibel( float linearVolume)
		{
			if( linearVolume > 0.0f)
			{
				return 20.0f * Mathf.Log10( linearVolume);
			}
			return -kMinimumDecibel;
		}
		const float kMinimumDecibel = 80.0f; // Mathf.Abs( -80.0f);
		
		[SerializeField]
		AudioMixer m_AudioMixer;
		[SerializeField]
		AudioMixerGroup m_BackgroundMixerGroup;
		
		static AudioClip s_CurrentBackgroundClip;
		static AudioSource s_CurrentBackgroundSource;
	}
}