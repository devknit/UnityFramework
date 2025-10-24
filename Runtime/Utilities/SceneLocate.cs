
using System;
using UnityEngine;

namespace Knit.Framework
{
	[Serializable]
	public sealed class SceneLocate
	{
		public string Value
		{
			get { return m_Value; }
		}
		public static implicit operator string( SceneLocate locate)
		{
			return locate.Value;
		}
		[SerializeField, SceneSelector( SceneNoun.Locate)]
		string m_Value;
	}
}