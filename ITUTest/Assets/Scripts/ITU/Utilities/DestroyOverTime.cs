using UnityEngine;

namespace ITU.Utilities
{
	public class DestroyOverTime : MonoBehaviour
	{
		[SerializeField] private float time;

		private void Start()
		{
			Invoke(nameof(DestroyMe), time);
		}

		private void DestroyMe()
		{
			Destroy(gameObject);
		}
	}
}
