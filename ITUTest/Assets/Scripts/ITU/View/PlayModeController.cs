using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITU.Algorithms;
using ITU.Game.Entities;
using ITU.Game.Properties;
using ITU.Grid;
using ITU.Utilities;
using UnityEngine;

namespace ITU.View
{
	public class PlayModeController : MonoBehaviour
	{
		[SerializeField] private LayerMask groundMask;
		[SerializeField] private Tooltip tooltip;

		private Pathfinding.ResultTile[] _playerReachable;
		private Pathfinding.ResultTile[] _attackReachable;
		private int[] _paths;

		private int _currentHighlight;

		private void OnEnable()
		{
			Player.Instance.OnFinishExecution += OnPlayerFinishedExecution;
			RecalculatePlayerPathfinding();
		}

		private void Update()
		{
			if (_paths == null)
			{
				return;
			}

			var grid = GameProperties.Grid;
			Vector3 playerPosition = Player.Instance.transform.position;
			Vector3 enemyPosition = Enemy.Instance.transform.position;
			var playerIndex = grid.GetTileIndexFromWorldPosition(playerPosition.x, playerPosition.z);
			var enemyIndex = grid.GetTileIndexFromWorldPosition(enemyPosition.x, enemyPosition.z);

			Camera cam = CameraController.Instance.Camera;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit info, 1000, groundMask))
			{
				var tileView = info.transform.GetComponentInParent<TileView>();
				if (tileView != null)
				{
					if (_currentHighlight != tileView.Tile.IndexInGrid)
					{
						_currentHighlight = tileView.Tile.IndexInGrid;
						if (_currentHighlight == enemyIndex)
						{
							if (_attackReachable.Length == 0)
							{
								// out of range
								tooltip.SetMessage("Out of Range");
							}
							else
							{
								// get lowest cost tile
								Pathfinding.ResultTile lowestCostTile = _attackReachable.GetBestComparison((a, b) => TileComparison(a, b, playerIndex));
								if (lowestCostTile.Index == playerIndex) // no move required
								{
									Player.Instance.SetShot((playerIndex, enemyIndex));
									Player.Instance.SetPath(null);
								}
								else // move and fire
								{
									var path = ConstructPath(playerIndex, lowestCostTile.Index);
									Player.Instance.SetPath(path);
									Player.Instance.SetShot((lowestCostTile.Index, enemyIndex));
								}

								tooltip.SetMessage(null);
							}
						}
						else if (_playerReachable.Any(tile => tile.Index == tileView.Tile.IndexInGrid))
						{
							var path = ConstructPath(playerIndex, tileView.Tile.IndexInGrid);
							Player.Instance.SetPath(path);
							Player.Instance.SetShot(null);

							tooltip.SetMessage(null);
						}
						else
						{
							tooltip.SetMessage("Invalid target");
						}
					}

					if (Input.GetMouseButtonDown(0) && !BlockUI.UIPointerBlocked)
					{
						if (Player.Instance.ExecutePath())
						{
							ClearPlayerPathfinding();
						}
					}
				}
			}
		}

		private void OnDisable()
		{
			ClearPlayerPathfinding();
			if (Player.Instance != null)
			{
				Player.Instance.OnFinishExecution -= OnPlayerFinishedExecution;
			}
		}

		private bool TileComparison(Pathfinding.ResultTile a, Pathfinding.ResultTile b, int playerIndex)
		{
			// var grid = GameProperties.Grid;
			// Vector2 aPos = grid.GetWorldPositionFromTileIndex(a.Index);
			// Vector2 bPos = grid.GetWorldPositionFromTileIndex(b.Index);
			// Vector2 playerPos = grid.GetWorldPositionFromTileIndex(playerIndex);
			
			return a.Cost < b.Cost /*&& Vector2.Distance(aPos, playerPos) < Vector2.Distance(bPos, playerPos)*/;
		}

		private List<int> ConstructPath(int from, int to)
		{
			var path = new List<int>();
			var current = _paths[to];
			path.Add(to);

			while (current != from)
			{
				path.Add(current);
				current = _paths[current];
			}

			path.Reverse();
			return path;
		}

		private async void RecalculatePlayerPathfinding()
		{
			ClearPlayerPathfinding();

			if (Player.Instance == null)
			{
				return;
			}

			Vector3 playerPos = Player.Instance.transform.position;
			Vector3 enemyPos = Enemy.Instance.transform.position;
			var i = GameProperties.Grid.GetTileIndexFromWorldPosition(playerPos.x, playerPos.z);
			var e = GameProperties.Grid.GetTileIndexFromWorldPosition(enemyPos.x, enemyPos.z);

			var floodFill = CalculateFloodFill(i);
			var dijkstra = CalculateDijkstraRaycast(e);

			var (playerReachable, playerPaths) = await floodFill;
			_playerReachable = playerReachable;
			_paths = playerPaths;

			var (attackFill, _) = await dijkstra;

			var list = new List<Pathfinding.ResultTile>();
			foreach (var a in attackFill)
			{
				Pathfinding.ResultTile found = playerReachable.FirstOrDefault(tile => tile.Index == a);
				if (found != null)
				{
					list.Add(found);
				}
			}

			_attackReachable = list.ToArray();

			UpdateViews();
		}

		private void UpdateViews()
		{
			var views = GameProperties.Views;
			foreach (Pathfinding.ResultTile tile in _playerReachable)
			{
				views[tile.Index].SetHighlight(TileView.Highlight.Blue);
			}
		}

		private void ClearPlayerPathfinding()
		{
			if (_playerReachable != null)
			{
				foreach (Pathfinding.ResultTile tile in _playerReachable)
				{
					GameProperties.Views[tile.Index]?.SetHighlight(TileView.Highlight.None);
				}
			}

			_paths = null;
			_playerReachable = null;
			_attackReachable = null;
			if (Player.Instance != null)
			{
				Player.Instance.SetPath(null);
				Player.Instance.SetShot(null);
			}
		}

		private void OnPlayerFinishedExecution()
		{
			RecalculatePlayerPathfinding();
		}

		private async Task<(Pathfinding.ResultTile[], int[])> CalculateFloodFill(int start)
		{
			return await Task.Run(
				() =>
				{
					return Pathfinding.FloodFill(
						GameProperties.Grid,
						start,
						GameProperties.MoveRange.Value * Grid1D<Tile>.NORMAL_COST,
						i => GameProperties.Grid[i].Type == TileType.Walkable);
				});
		}

		private async Task<(int[], int[])> CalculateDijkstraRaycast(int start)
		{
			return await Task.Run(
				() =>
				{
					return Pathfinding.DijkstraRaycast(
						GameProperties.Grid,
						start,
						GameProperties.AttackRange.Value * Grid1D<Tile>.NORMAL_COST,
						i => GameProperties.Grid[i].Type == TileType.Walkable ||
							GameProperties.Grid[i].Type == TileType.Cover);
				});
		}
	}
}
