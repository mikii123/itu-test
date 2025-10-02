using System;
using UnityEngine;

namespace ITU.Utilities
{
	[Serializable]
	public abstract class Singleton<T> : MonoBehaviour
		where T : Singleton<T>
	{
		public static T Instance
		{
			get => instance ?? (instance = FindObjectOfType<T>());
			protected set => instance = value;
		}

		protected static T instance;

		protected virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		protected virtual void OnApplicationQuit()
		{
			Instance = null;
		}

		public virtual void Init()
		{ }
	}
}
