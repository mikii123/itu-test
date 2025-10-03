using System.Linq;
using System.Threading.Tasks;
using ITU.Algorithms;
using ITU.Game.Properties;
using ITU.Grid;
using LELWare.Initialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITU.Game.Init
{
	public class SetupGridViewInitStep : InitStep
	{
		public override async Task Startup()
		{
			var grid = GameProperties.Grid;
			var handles = new AsyncOperationHandle<GameObject>[grid.Length];
			var views = new Grid1D<TileView>(GameProperties.GridProperties.Value);
			GameProperties.Views = views;

			foreach (Tile tile in grid)
			{
				handles[tile.IndexInGrid] = Addressables.InstantiateAsync("TileView", GameEntry.Instance.transform);
			}

			await Task.WhenAll(handles.Select(handle => handle.Task));

			for (var i = 0; i < views.Length; i++)
			{
				var tileView = handles[i].Result.GetComponent<TileView>();
				tileView.Setup(grid[i]);
				GameProperties.Views[i] = tileView;
			}
		}

		public override Task Shutdown()
		{
			foreach (TileView view in GameProperties.Views)
			{
				Object.Destroy(view.gameObject);
			}

			return Task.CompletedTask;
		}
	}
}
