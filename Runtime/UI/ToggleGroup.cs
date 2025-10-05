
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Knit.Framework
{
	public class ToggleGroup : UIBehaviour
	{
		public IReadOnlyList<Toggle> Elements
		{
			get{ return m_Toggles; }
		}
		internal bool RegisterToggle( Toggle toggle)
		{
			if( m_Toggles.Contains( toggle) == false)
			{
				m_Toggles.Add( toggle);
				// m_Toggles.Sort( Compare);
				return true;
			}
			return false;
		}
		internal void UnregisterToggle( Toggle toggle)
		{
			if( m_Toggles.Contains( toggle) != false)
			{
				m_Toggles.Remove( toggle);
			}
		}
		internal bool Normalize( Toggle pivot, bool sendCallback)
		{
			if( m_AwakeOnEvent != false)
			{
				sendCallback = true;
			}
			if( m_Toggles.Count > 0)
			{
				if( (pivot?.m_IsOn ?? false) != false)
				{
					for( int i0 = 0; i0 < m_Toggles.Count; ++i0)
					{
						if( m_Toggles[ i0] != pivot
						&&	m_Toggles[ i0].m_IsOn != false)
						{
							m_Toggles[ i0].SetWithoutGroup( false, sendCallback);
						}
					}
					return m_AwakeOnEvent;
				}
				else
				{
					Toggle toggle;
					int count = 0;
					int index = -1;
					
					for( int i0 = 0; i0 < m_Toggles.Count; ++i0)
					{
						toggle = m_Toggles[ i0];
						
						if( toggle.m_IsOn != false)
						{
							if( count > 0)
							{
								toggle.SetWithoutGroup( false, sendCallback);
							}
							++count;
						}
						else if( toggle == pivot)
						{
							index = i0;
						}
					}
					if( m_AllowSwitchOff == false && count == 0)
					{
						index = (index < 0)? 0 : (index + 1) % m_Toggles.Count;
						m_Toggles[ index].SetWithoutGroup( true, sendCallback);
					}
				}
			}
			return false;
		}
		internal Toggle FindOnToggle()
		{
			return m_Toggles.FirstOrDefault( x => x.IsOn != false);
		}
	#if UNITY_EDITOR
		protected override void Reset()
		{
			CollectionsInChildren();
		}
		internal void CollectionsInChildren()
		{
			m_Toggles = gameObject.GetComponentsInChildren<Toggle>( true).ToList();
		}
	#endif
		static int Compare( Toggle a, Toggle b)
		{
			var aTransform = a.transform as RectTransform;
			var bTransform = b.transform as RectTransform;
			int aValue = (int)(aTransform.anchoredPosition.x + aTransform.anchoredPosition.y * 2048);
			int bValue = (int)(bTransform.anchoredPosition.x + bTransform.anchoredPosition.y * 2048);
			return aValue - bValue;
		} 
		[SerializeField]
		bool m_AllowSwitchOff;
		[SerializeField]
		bool m_AwakeOnEvent;
		[SerializeField]
        List<Toggle> m_Toggles;
	}
}
