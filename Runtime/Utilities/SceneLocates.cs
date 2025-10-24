
using System;
using UnityEngine;

namespace Knit.Framework
{
	[Serializable]
	public sealed class SceneLocates
	{
		public string[] Values
		{
			get { return m_Values; }
		}
		public static implicit operator string[]( SceneLocates locates)
		{
			return locates.Values;
		}
		[SerializeField, SceneSelector( SceneNoun.Locate)]
		string[] m_Values;
	}
}