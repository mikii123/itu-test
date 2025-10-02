using ITU.Game.Properties;
using UnityEngine;

namespace ITU.View
{
	public class PlayModeController : MonoBehaviour
	{
		private void Start()
		{
			GameProperties.MoveRange.OnChange += MoveRangeOnChange;
			GameProperties.AttackRange.OnChange += AttackRangeOnChange;
		}

		private void OnDestroy()
		{
			GameProperties.MoveRange.OnChange -= MoveRangeOnChange;
			GameProperties.AttackRange.OnChange -= AttackRangeOnChange;
		}

		private void AttackRangeOnChange(int value)
		{
			
		}

		private void MoveRangeOnChange(int value)
		{
			
		}
	}
}
