
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;

namespace Knit.Framework.Sample
{
	public sealed class EntryPoint : MonoBehaviour
	{
		IEnumerator Start()
		{
			SceneInstance? sceneInstance = null;
			Color fadeColor = m_Fade.color;
			const float duration = 1.0f;
			float elapsedTime = 0.0f;
			
			SceneHandle.Boot( m_BootScene).Completed += (handle) =>
			{
				sceneInstance = handle.Result;
			};
			
			// フェードアウト
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				fadeColor.a = 1.0f - Mathf.Clamp01( elapsedTime / duration);
				m_Fade.color = fadeColor;
				yield return null;
			}
			fadeColor.a = 0.0f;
			elapsedTime = 0.0f;
			
			// 最低限の表示時間分を待ちつつ、読み込み完了を待つ
			while( sceneInstance.HasValue == false || elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			elapsedTime = 0.0f;
			
			// フェードイン
			while( elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				
				if( elapsedTime >= duration)
				{
					break;
				}
				fadeColor.a = Mathf.Clamp01( elapsedTime / duration);
				m_Fade.color = fadeColor;
				yield return null;
			}
			fadeColor.a = 1.0f;
			
			yield return sceneInstance.Value.ActivateAsync();
		}
		[SerializeField]
		AssetReferenceScene m_BootScene;
		[SerializeField]
		Image m_Fade;
	}
}