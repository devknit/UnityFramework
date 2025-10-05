
using UnityEngine;
using System.Collections;

namespace Knit.Framework.Sample
{
	public sealed class YellowContext : ModalContext
	{
		public YellowContext( System.Action<bool> callback) : base( "Yellow")
		{
			m_Callback = callback;
		}
		protected override IEnumerator OnPreLoad()
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnPreLoad\n");
			yield break;
		}
		protected override IEnumerator OnPrepare( System.Action onFailure)
		{
			Debug.Log( $"<color=yellow>{GetType().Name}: OnPrepare\n");
			yield break;
		}
		internal System.Action<bool> Callback
		{
			get{ return m_Callback; }
		}
        readonly System.Action<bool> m_Callback;
	}
}