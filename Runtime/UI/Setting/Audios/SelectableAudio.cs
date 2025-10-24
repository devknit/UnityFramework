
using UnityEngine;

namespace Knit.Framework
{
	public abstract class SelectableAudio : ScriptableObject
	{
		protected internal abstract SelectableAudioContext Play( SelectionState state);
		protected internal abstract void Stop( SelectableAudioContext context);
		
		internal string AudioName
		{
			get{ return m_AudioName; }
		}
		[SerializeField]
		string m_AudioName;
	}
}
