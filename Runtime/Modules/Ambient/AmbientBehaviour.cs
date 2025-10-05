
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Framework
{
	[Serializable]
	sealed class AmbientBehaviour : PlayableBehaviour
	{
		[SerializeField, SceneSelector( SceneNoun.Locate)]
		internal string m_AmbientScene;
	}
}
