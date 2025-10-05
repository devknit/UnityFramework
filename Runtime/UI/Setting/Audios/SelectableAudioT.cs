
using UnityEngine;

namespace Knit.Framework
{
	public abstract class SelectableAudioT<TStateAudio, TAudioContext> : SelectableAudio
		where TStateAudio : SelectableStateAudio
		where TAudioContext : SelectableAudioContext
	{
		protected bool TryGetStateAudio( SelectionState state, out TStateAudio audio)
		{
			audio = state switch
			{
				SelectionState.Disabled => m_Disable,
				SelectionState.Normal => m_Normal,
				SelectionState.Highlighted => m_Highlighte,
				SelectionState.Selected => m_Select,
				SelectionState.Pressed => m_Presse,
				SelectionState.Submited => m_Submit,
				_ => null
			};
			return audio?.IsValid() ?? false;
		}
		[SerializeField]
		TStateAudio m_Disable;
		[SerializeField]
		TStateAudio m_Normal;
		[SerializeField]
		TStateAudio m_Highlighte;
		[SerializeField]
		TStateAudio m_Select;
		[SerializeField]
		TStateAudio m_Presse;
		[SerializeField]
		TStateAudio m_Submit;
	}
}
