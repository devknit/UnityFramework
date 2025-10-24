
using System.Collections;

namespace Knit.Framework
{
	public abstract partial class SceneHandle : ModuleHandle
	{
		protected private abstract SceneContext OnSceneContext( SceneContext sceneContext);
		
		/// <summary>
		/// Start() 期間中にアセット読み込み完了待ち前に呼び出されます
		/// </summary>
		/// <param name="onFailure">モジュールを中断したい場合に呼び出します</param>
		protected virtual IEnumerator OnPrepare( System.Action onFailure)
		{
			yield break;
		}
		/// <summary>
		/// Start() 期間中にアセット読み込み完了時に呼び出されます
		/// </summary>
		/// <param name="onFailure">モジュールを中断したい場合に呼び出します</param>
		protected virtual IEnumerator OnSetup( System.Action onFailure)
		{
			yield break;
		}
		/// <summary>
		/// Start() 期間中に画面の遷移演出のために呼び出されます
		/// </summary>
		protected virtual IEnumerator OnOpen()
		{
			yield break;
		}
		/// <summary>
		/// Start() 期間中に画面の開始直前処理を行うために呼び出されます
		/// </summary>
		protected virtual IEnumerator OnStart()
		{
			yield break;
		}
		/// <summary>
		/// 画面の停止処理を行うために呼び出されます
		/// </summary>
		/// <param name="requestSceneContext">移行予定のコンテキスト</param>
		/// <param name="onFailure">移行を中断したい場合に呼び出します</param>
		protected virtual IEnumerator OnStop( SceneContext requestSceneContext, System.Action onFailure)
		{
			yield break;
		}
		/// <summary>
		/// 画面の遷移演出のために呼び出されます
		/// </summary>
		protected virtual IEnumerator OnClose()
		{
			yield break;
		}
		/// <summary>
		/// 遷移予定の画面が中断された場合に呼び出されます
		/// </summary>
		protected virtual IEnumerator OnResume( SceneContext abortSceneContext)
		{
			yield break;
		}
		protected virtual bool OnInactivate()
		{
			return true;
		}
		protected virtual IEnumerator OnPreUnload()
		{
			yield break;
		}
	}
}