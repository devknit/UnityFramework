
using UnityEngine;

namespace Knit.Framework
{
	public class SceneSelectorAttribute : PropertyAttribute
	{
		public SceneSelectorAttribute( SceneNoun noun)  
		{  
			m_Noun = noun;
		}
		public SceneNoun Noun
		{
			get { return m_Noun; }
		}
		readonly SceneNoun m_Noun;
	}
}