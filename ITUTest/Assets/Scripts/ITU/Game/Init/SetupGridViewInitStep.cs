using System.Linq;
using System.Threading.Tasks;
using ITU.Algorithms;
using ITU.Game.Properties;
using LELWare.Initialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITU.Game.Init
{
	public class SetupGridViewInitStep : InitStep
	{
		private TileView[] _viewsTab;

		public override async Task Startup()
		{
			var grid = GameProperties.Grid;
			var handles = new AsyncOperationHandle<GameObject>[grid.Length];
			_viewsTab = new TileView[grid.Length];

			foreach (Tile tile in grid)
			{
				handles[tile.IndexInGrid] = Addressables.InstantiateAsync("TileView", GameEntry.Instance.transform);
			}

			await Task.WhenAll(handles.Select(handle => handle.Task));

			for (var i = 0; i < _viewsTab.Length; i++)
			{
				var tileView = handles[i].Result.GetComponent<TileView>();
				tileView.Setup(grid[i]);
				_viewsTab[i] = tileView;
			}
		}

		public override Task Shutdown()
		{
			foreach (TileView view in _viewsTab)
			{
				Object.Destroy(view.gameObject);
			}

			return Task.CompletedTask;
		}
	}
}
