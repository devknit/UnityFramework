
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Framework
{
	[TrackClipType( typeof( AmbientClip))]
	[TrackColor( 252.0f / 255.0f, 252.0f / 255.0f, 252.0f / 255.0f)]
	[DisplayName( "Knit.Timeline/Ambient Track")]
	sealed class AmbientTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<AmbientMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
