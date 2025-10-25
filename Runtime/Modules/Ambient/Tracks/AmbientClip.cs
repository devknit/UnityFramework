
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[System.Serializable]
	sealed class AmbientClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<AmbientBehaviour>.Create( graph, m_Source);
		}
		[SerializeField]
		AmbientBehaviour m_Source = new();
	}
}
