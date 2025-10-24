
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Knit.Framework
{
	public class ModuleSetting : ScriptableObject
	{
		protected virtual IEnumerator OnInitialize()
		{
			yield break;
		}
		internal IEnumerator Initialize()
		{
			if( m_IsInitialized == false)
			{
				InstantiateDaemons();
				
				IEnumerator it = OnInitialize();
				
				while( it.MoveNext() != false)
				{
					yield return it.Current;
				}
				m_IsInitialized = true;
			}
		}
		void InstantiateDaemons()
		{
			if( m_DaemonPrefabs?.Length > 0)
			{
				m_DaemonInstances = new GameObject[ m_DaemonPrefabs.Length];
				
				for( int i0 = 0; i0 < m_DaemonPrefabs.Length; ++i0)
				{
					GameObject prefab = m_DaemonPrefabs[ i0];
					
					if( prefab != null)
					{
						if( Instantiate( prefab, null, false) is GameObject instance)
						{
							instance.name = prefab.name;
							m_DaemonInstances[ i0] = instance;
						}
					}
				}
			}
		}
		internal bool TryGetModalModulePrefab( string key, out ModalHandle modalPrefab)
		{
			return m_ModalPrefabs.TryGetValue( key, out modalPrefab);
		}
		public void SetScreenNeverSleep( bool enable)
		{
			if( enable != false)
			{
				m_ScreenNeverSleep |= ScreenNeverSleep.Module;
			}
			else
			{
				m_ScreenNeverSleep &= ~ScreenNeverSleep.Module;
			}
			UpdateScreenNeverSleep();
		}
		internal void SetScreenNeverSleepInternal( bool enable)
		{
			if( enable != false)
			{
				m_ScreenNeverSleep |= ScreenNeverSleep.Inernal;
			}
			else
			{
				m_ScreenNeverSleep &= ~ScreenNeverSleep.Inernal;
			}
			UpdateScreenNeverSleep();
		}
		void UpdateScreenNeverSleep()
		{
			if( m_ScreenNeverSleep != ScreenNeverSleep.None)
			{
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
			}
			else
			{
				Screen.sleepTimeout = SleepTimeout.SystemSetting;
			}
		}
		internal bool TryGetSelectableActions( int index, out SelectableAction selectableAction)
		{
			if( index < 0 || index >= m_SelectableActions.Count)
			{
				selectableAction = null;
				return selectableAction != null;
			}
			selectableAction = m_SelectableActions[ index];
			return selectableAction != null;
		}
		internal bool TryGetSelectableAudios( int index, out SelectableAudio selectableAudio)
		{
			if( index < 0 || index >= m_SelectableAudios.Count)
			{
				selectableAudio = null;
				return selectableAudio != null;
			}
			selectableAudio = m_SelectableAudios[ index];
			return selectableAudio != null;
		}
		internal IReadOnlyList<SelectableAction> SelectableActions
		{
			get{ return m_SelectableActions; }
		}
		internal IReadOnlyList<SelectableAudio> SelectableAudios
		{
			get{ return m_SelectableAudios; }
		}
		enum ScreenNeverSleep
		{
			None = 0,
			Module = 1 << 0,
			Inernal = 1 << 1,
		}
		[System.Serializable]
		sealed class ModalPrefabs : SerializedDictionary<string, ModalHandle>
		{
		}
		[System.NonSerialized]
		bool m_IsInitialized;
		[SerializeField]
		GameObject[] m_DaemonPrefabs;
		[System.NonSerialized]
		GameObject[] m_DaemonInstances;
		[SerializeField]
		ModalPrefabs m_ModalPrefabs;
		[SerializeField]
		List<SelectableAction> m_SelectableActions;
		[SerializeField]
		List<SelectableAudio> m_SelectableAudios;
		[System.NonSerialized]
		ScreenNeverSleep m_ScreenNeverSleep;
	}
}