
using UnityEngine;

namespace Knit.Framework
{
	public enum NavigateMode
	{
		None = 0,
		Explicit = 1,
		Automatic = 2,
	}
	[System.Serializable]
	public struct Navigate : System.IEquatable<Navigate>
	{
		public NavigateMode Mode{ readonly get { return m_Mode; } set { m_Mode = value; } }
		public Selectable Target{ readonly get { return m_Target; } set { m_Target = value; } }
		public bool WrapAround{ readonly get { return m_WrapAround; } set { m_WrapAround = value; } }
		public float Correction{ readonly get { return m_Correction; } set { m_Correction = value; } }
		
		public readonly bool IsValid()
		{
			return m_Mode switch
			{
				NavigateMode.Explicit => true, // m_Target != null,
				NavigateMode.Automatic => true,
				_ => false
			};
		}
		public readonly bool Equals( Navigate other)
		{
			return	m_Mode == other.m_Mode
				&&	m_Target == other.m_Target
				&&	m_WrapAround == other.m_WrapAround
				&&	m_Correction == other.m_Correction;
		}
		public static Navigate DefaultNavigation
		{
			get
			{
				return new Navigate
				{
					m_Mode = NavigateMode.Automatic,
					m_Correction = 1.0f,
					m_WrapAround = false,
				};
			}
		}
		[SerializeField]
		NavigateMode m_Mode;
		[SerializeField]
		Selectable m_Target;
		[SerializeField, Min( 0.001f)]
		float m_Correction;
		[SerializeField]
		bool m_WrapAround;
	}
}
