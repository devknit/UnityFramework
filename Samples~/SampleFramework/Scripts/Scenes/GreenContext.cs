
using UnityEngine;
using System.Collections;

namespace Knit.Framework.Sample
{
	public sealed class GreenContext : SceneContext
	{
		public GreenContext() : base( "Green")
		{
		}
		protected override IEnumerator OnPreLoad()
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnPreLoad\n");
			LoadAmbientSceneAsync( "Daytime");
			AmbientModule.Forcus( "Daytime");
			yield break;
		}
		protected override IEnumerator OnPrepare( System.Action onFailure)
		{
			Debug.Log( $"<color=green>{GetType().Name}: OnPrepare\n");
			yield break;
		}
	}
}