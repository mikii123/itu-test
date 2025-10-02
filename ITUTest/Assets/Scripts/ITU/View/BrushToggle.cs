using UnityEngine;

namespace ITU.View
{
	public class BrushToggle: MonoBehaviour
	{
		[SerializeField] private EditModeController.BrushType type;
		[SerializeField] private EditModeController controller;
		
		public void SetToggle(bool value)
		{
			controller.OnSwitchBrush(value ? type : EditModeController.BrushType.None);
		}
	}
}
