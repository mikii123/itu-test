using LELWare.Initialization;
using UnityEngine;

namespace ITU.Game.Init
{
	public class GameEntry : MonoBehaviour
	{
		public static GameEntry Instance { get; private set; }

		private IInitSystem _initSystem;
		private GameEntrySteps _steps = new();

		private void Awake()
		{
			Instance = this;
		}

		private async void Start()
		{
			_initSystem = new InitSystem();
			await _initSystem.Startup(_steps);
		}

		private async void OnDestroy()
		{
			await _initSystem.Shutdown(_steps);
		}
	}
}
