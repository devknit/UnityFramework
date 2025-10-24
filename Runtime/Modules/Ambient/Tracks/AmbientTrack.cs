
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[TrackClipType( typeof( AmbientClip))]
	[TrackColor( 252.0f / 255.0f, 252.0f / 255.0f, 252.0f / 255.0f)]
	sealed class AmbientTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<AmbientMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
