
using UnityEngine.Playables;
using Knit.Framework;

namespace Knit.Timeline
{
	sealed class AmbientMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( m_DefaultAmbient == null)
			{
				m_CachedAmbient = m_DefaultAmbient = AmbientModule.GetForcus();
			}
			int inputCount = playable.GetInputCount();
			string ambientScene = m_DefaultAmbient;
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				if( playable.GetInputWeight( i0) > 0.0f)
				{
					var inputPlayable = (ScriptPlayable<AmbientBehaviour>)playable.GetInput( i0);
					ambientScene = inputPlayable.GetBehaviour().m_AmbientScene;
				}
			}
			if( m_CachedAmbient != ambientScene)
			{
				if( AmbientModule.Forcus( ambientScene) != false)
				{
					m_CachedAmbient = ambientScene;
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_DefaultAmbient != null)
			{
				AmbientModule.Forcus( m_DefaultAmbient);
			}
		}
		string m_DefaultAmbient;
		string m_CachedAmbient;
	}
}
