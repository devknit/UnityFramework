
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Knit.Framework
{
	internal sealed class AudioSourcePool : MonoBehaviour
	{
		public static AudioSourcePool Instance
		{
			get;
			private set;
		}
		public AudioSource Play( AudioClip clip, AudioMixerGroup mixerGroup, float volume, bool loop)
		{
			if( m_LastCheckFrame != Time.frameCount)
			{
				m_LastCheckFrame = Time.frameCount;
				CheckInUse();
			}
			AudioSource audioSource = (m_Pool.Count == 0)?
				Instantiate( m_Prefab, transform, false) : m_Pool.Dequeue();
			
			if( m_NodePool.Count == 0)
			{
				m_Use.AddLast( audioSource);
			}
			else
			{
				var node = m_NodePool.Dequeue();
				node.Value = audioSource;
				m_Use.AddLast( node);
			}
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = volume;
			audioSource.loop = loop;
			audioSource.clip = clip;
			audioSource.Play();
			return audioSource;
		}
		void CheckInUse()
		{
			var node = m_Use.First;
			
			while( node != null)
			{
				var current = node;
				node = node.Next;
				
				if( current.Value.isPlaying == false)
				{
					m_Pool.Enqueue( current.Value);
					m_Use.Remove( current);
					m_NodePool.Enqueue(current);
				}
			}
		}
		void Awake()
		{
			if( Instance != null)
			{
				Destroy( gameObject);
			}
			else
			{
				Instance = this;
				
				if( transform.parent == null)
				{
					DontDestroyOnLoad( gameObject);
				}
			}
		}
		[SerializeField]
		AudioSource m_Prefab;
		int m_LastCheckFrame = -1;
		readonly Queue<AudioSource> m_Pool = new();
		readonly LinkedList<AudioSource> m_Use = new();
		readonly Queue<LinkedListNode<AudioSource>> m_NodePool = new();
	}
}
