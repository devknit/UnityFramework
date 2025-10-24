
using UnityEngine;
using System.Collections;

namespace Knit.Framework.Sample
{
	public sealed class RedContext : SceneContext
	{
		public RedContext() : base( "Red")
		{
		}
		protected override IEnumerator OnPreLoad()
		{
			Debug.Log( $"<color=red>{GetType().Name}: OnPreLoad\n");
			LoadAmbientSceneAsync( "Evening");
			AmbientModule.Forcus( "Evening");
			yield break;
		}
		protected override IEnumerator OnPrepare( System.Action onFailure)
		{
			Debug.Log( $"<color=red>{GetType().Name}: OnPrepare\n");
			yield break;
		}
	}
}