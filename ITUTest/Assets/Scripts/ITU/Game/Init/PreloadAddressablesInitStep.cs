using System.Threading.Tasks;
using LELWare.Initialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITU.Game.Init
{
	public class PreloadAddressablesInitStep : InitStep
	{
		private AsyncOperationHandle<GameObject> handle;
		public override async Task Startup()
		{
			handle = Addressables.LoadAssetAsync<GameObject>("TileView");
			await handle.Task;
		}

		public override Task Shutdown()
		{
			Addressables.Release(handle);
			return Task.CompletedTask;
		}
	}
}
