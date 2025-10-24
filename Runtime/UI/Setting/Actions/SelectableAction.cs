
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Knit.Framework
{
	public abstract class SelectableAction : ScriptableObject
	{
		public string ActionName
		{
			get
			{
				if( string.IsNullOrWhiteSpace( m_ActionName) != false)
				{
					m_ActionName = GetDefaultActionName();
				}
				return m_ActionName;
			}
		}
		internal abstract void SetValue( 
			SelectionState state, Graphic graphic, Transform transform);
		internal abstract IEnumerator OnTween( 
			SelectionState state,Graphic graphic, Transform transform,  
			bool ignoreTimeScale, System.Action<SelectionState> onActionCompleted);
		protected abstract string GetDefaultActionName();
		
		[SerializeField]
		string m_ActionName;
	}
}
