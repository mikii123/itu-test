using System.Threading.Tasks;
using LELWare.Initialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITU.Game.Init
{
	public class PreloadAddressablesInitStep : InitStep
	{
		private AsyncOperationHandle<GameObject> tileHandle;
		private AsyncOperationHandle<GameObject> playerHandle;
		private AsyncOperationHandle<GameObject> enemyHandle;
		private AsyncOperationHandle<GameObject> shotHandle;
		private AsyncOperationHandle<GameObject> explosionHandle;

		public override async Task Startup()
		{
			tileHandle = Addressables.LoadAssetAsync<GameObject>("TileView");
			playerHandle = Addressables.LoadAssetAsync<GameObject>("Player");
			enemyHandle = Addressables.LoadAssetAsync<GameObject>("Enemy");
			shotHandle = Addressables.LoadAssetAsync<GameObject>("Shot");
			explosionHandle = Addressables.LoadAssetAsync<GameObject>("Explosion");
			
			await Task.WhenAll(tileHandle.Task, playerHandle.Task, enemyHandle.Task, shotHandle.Task, explosionHandle.Task);
		}

		public override Task Shutdown()
		{
			Addressables.Release(tileHandle);
			Addressables.Release(playerHandle);
			Addressables.Release(enemyHandle);
			Addressables.Release(shotHandle);
			Addressables.Release(explosionHandle);
			
			return Task.CompletedTask;
		}
	}
}
