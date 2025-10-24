#if ENABLE_INPUT_SYSTEM

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Knit.Framework
{
	[Serializable]
	internal struct InputActionSet : IEquatable<InputActionSet>, IEquatable<InputAction>, IEquatable<InputActionReference>
	{
		public InputAction Action
		{
			get{ return m_UseReference ? m_Reference != null ? m_Reference.action : null : m_Action; }
		}
		public InputActionReference Reference
		{
			get{ return m_UseReference ? m_Reference : null; }
		}
		public bool IgnoreInteractable
		{
			get{ return m_IgnoreInteractable; }
		}
		public InputActionSet( InputAction action, bool ignoreInteractable)
		{
			m_UseReference = false;
			m_Action = action;
			m_Reference = null;
			m_IgnoreInteractable = ignoreInteractable;
		}
		public InputActionSet( InputActionReference reference, bool ignoreInteractable)
		{
			m_UseReference = true;
			m_Action = null;
			m_Reference = reference;
			m_IgnoreInteractable = ignoreInteractable;
		}
		public bool Equals( InputActionSet other)
		{
			return m_Reference == other.m_Reference &&
				m_UseReference == other.m_UseReference &&
				m_Action == other.m_Action &&
				m_IgnoreInteractable == other.m_IgnoreInteractable;
		}
		public bool Equals( InputAction other)
		{
			return ReferenceEquals( Action, other);
		}
		public bool Equals( InputActionReference other)
		{
			return m_Reference == other;
		}
		public override bool Equals( object obj)
		{
			if( m_UseReference != false)
			{
				return Equals( obj as InputActionReference);
			}
			return Equals( obj as InputAction);
		}
		public override int GetHashCode()
		{
			if( m_UseReference != false)
			{
				return (m_Reference != null)? m_Reference.GetHashCode() : 0;
			}
			return (m_Action != null)? m_Action.GetHashCode() : 0;
		}
		public static bool operator==( InputActionSet left, InputActionSet right)
		{
			return left.Equals( right);
		}
		public static bool operator!=( InputActionSet left, InputActionSet right)
		{
			return !left.Equals( right);
		}
		[SerializeField]
		bool m_UseReference;
		[SerializeField]
		InputAction m_Action;
		[SerializeField]
		InputActionReference m_Reference;
		[SerializeField]
		bool m_IgnoreInteractable;
	}
}
#endif
