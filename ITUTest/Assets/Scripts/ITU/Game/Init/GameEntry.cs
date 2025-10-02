using System.Threading.Tasks;
using LELWare.Initialization;
using UnityEngine;

namespace ITU.Game.Init
{
	public class GameEntry : MonoBehaviour
	{
		public static GameEntry Instance { get; private set; }

		private IInitSystem _initSystem;
		private GameEntrySteps _steps = new();

		private async void Awake()
		{
			Instance = this;

			_initSystem = new InitSystem();
			await _initSystem.Startup(_steps);
		}

		private async void OnDestroy()
		{
			await _initSystem.Shutdown(_steps);
		}

		public async Task Reload()
		{
			await _initSystem.Shutdown(_steps);
			await _initSystem.Startup(_steps);
		}
	}
}
