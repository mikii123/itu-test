using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ITU.Utilities
{
	public class BlockUI: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public static bool UIPointerBlocked { get; set; }

		private void OnDisable()
		{
			UIPointerBlocked = false;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			UIPointerBlocked = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			UIPointerBlocked = false;
		}
	}
}
