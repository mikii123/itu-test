using TMPro;
using UnityEngine;

namespace ITU.View
{
	public class Tooltip : MonoBehaviour
	{
		[SerializeField] private TMP_Text text;
		
		private RectTransform _transform;

		private void Awake()
		{
			_transform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			_transform.position = Input.mousePosition;
		}

		public void SetMessage(string message)
		{
			if (message == null)
			{
				gameObject.SetActive(false);
				return;
			}

			text.text = message;
			gameObject.SetActive(true);
		}
	}
}
