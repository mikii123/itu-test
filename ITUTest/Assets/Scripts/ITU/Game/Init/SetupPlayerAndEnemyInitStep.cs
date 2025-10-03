using System.Threading.Tasks;
using ITU.Algorithms;
using ITU.Game.Properties;
using LELWare.Initialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITU.Game.Init
{
	public class SetupPlayerAndEnemyInitStep : InitStep
	{
		private GameObject _player;
		private GameObject _enemy;
		
		public override async Task Startup()
		{
			AsyncOperationHandle<GameObject> playerHandle = new AsyncOperationHandle<GameObject>();
			AsyncOperationHandle<GameObject> enemyHandle = new AsyncOperationHandle<GameObject>();

			var grid = GameProperties.Grid;
			for (var i = 0; i < grid.Length; i++)
			{
				if (grid[i].Type != TileType.Walkable)
				{
					continue;
				}

				Vector2 position = grid.GetWorldPositionFromTileIndex(i) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
				var pos = new Vector3(position.x, 0, position.y);
				playerHandle = Addressables.InstantiateAsync("Player", pos, Quaternion.identity);
				
				break;
			}

			for (var i = GameProperties.Grid.Length - 1; i >= 0; i--)
			{
				if (grid[i].Type != TileType.Walkable)
				{
					continue;
				}

				Vector2 position = grid.GetWorldPositionFromTileIndex(i) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
				var pos = new Vector3(position.x, 0, position.y);
				enemyHandle = Addressables.InstantiateAsync("Enemy", pos, Quaternion.identity);
				
				break;
			}

			await Task.WhenAll(playerHandle.Task, enemyHandle.Task);
			
			_player = playerHandle.Result;
			_enemy = enemyHandle.Result;
		}

		public override Task Shutdown()
		{
			Object.Destroy(_player);
			Object.Destroy(_enemy);
			
			return Task.CompletedTask;
		}
	}
}
