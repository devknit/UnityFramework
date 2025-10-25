
using UnityEngine;
using UnityEngine.Playables;
using Knit.Framework;

namespace Knit.TimelineExtension
{
	[System.Serializable]
	sealed class AmbientBehaviour : PlayableBehaviour
	{
		[SerializeField, SceneSelector( SceneNoun.Locate)]
		internal string m_AmbientScene;
	}
}
