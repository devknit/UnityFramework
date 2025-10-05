
using UnityEngine;

namespace Knit.Framework
{
	public enum NavigationMode
	{
		None,
		Horizontal,
		Vertical,
		Automatic,
		Explicit,
	}
	[System.Serializable]
	public struct Navigation : System.IEquatable<Navigation>
	{
		public NavigationMode Mode{ readonly get { return m_Mode; } set { m_Mode = value; } }
		public bool WrapAround{ readonly get { return m_WrapAround; } set { m_WrapAround = value; } }
		public Selectable SelectOnUp{ readonly get { return m_SelectOnUp; } set { m_SelectOnUp = value; } }
		public Selectable SelectOnDown{ readonly get { return m_SelectOnDown; } set { m_SelectOnDown = value; } }
		public Selectable SelectOnLeft{ readonly get { return m_SelectOnLeft; } set { m_SelectOnLeft = value; } }
		public Selectable SelectOnRight{ readonly get { return m_SelectOnRight; } set { m_SelectOnRight = value; } }
		
		public readonly bool Equals( Navigation other)
		{
			return	m_Mode == other.m_Mode
				&&	m_SelectOnUp == other.m_SelectOnUp
				&&	m_SelectOnDown == other.m_SelectOnDown
				&&	m_SelectOnLeft == other.m_SelectOnLeft
				&&	m_SelectOnRight == other.m_SelectOnRight;
		}
		static public Navigation DefaultNavigation
		{
			get
			{
                var defaultNav = new Navigation
                {
                    m_Mode = NavigationMode.Automatic,
                    m_WrapAround = false
                };
                return defaultNav;
			}
		}
		[SerializeField]
		NavigationMode m_Mode;
		[SerializeField]
		bool m_WrapAround;
		[SerializeField]
		Selectable m_SelectOnUp;
		[SerializeField]
		Selectable m_SelectOnDown;
		[SerializeField]
		Selectable m_SelectOnLeft;
		[SerializeField]
		Selectable m_SelectOnRight;
	}
}
