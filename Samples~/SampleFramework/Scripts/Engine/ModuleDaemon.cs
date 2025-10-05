
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Knit.Framework.Sample
{
	public sealed class ModuleDaemon : Framework.ModuleDaemon
	{
		internal IEnumerator FadeIn( float duration)
		{
			Color color = m_FadeImage.color;
			float elapsedTime = 0.0f;
			
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				color.a = Mathf.Clamp01( elapsedTime / duration);
				m_FadeImage.color = color;
				yield return null;
			}
			color.a = 1.0f;
			m_FadeImage.color = color;
		}
		internal IEnumerator FadeOut( float duration)
		{
			Color color = m_FadeImage.color;
			float elapsedTime = 0.0f;
			
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				color.a = 1.0f - Mathf.Clamp01( elapsedTime / duration);
				m_FadeImage.color = color;
				yield return null;
			}
			color.a = 0.0f;
			m_FadeImage.color = color;
		}
		[SerializeField]
		UnityEngine.UI.Image m_FadeImage;
	}
}