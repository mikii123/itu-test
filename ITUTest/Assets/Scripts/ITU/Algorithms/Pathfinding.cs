using System;
using System.Collections.Generic;
using System.Linq;
using C5;
using ITU.Grid;
using UnityEngine;

namespace ITU.Algorithms
{
	public static class Pathfinding
	{
		/// <summary>
		///     Dijkstra implementation on indexes. Flood-fill version. No early out.
		/// </summary>
		public static (ResultTile[], int[]) FloodFill
		(
			Grid1D<Tile> grid,
			int startNode,
			int range,
			Func<int, bool> isWalkable)
		{
			var frontier = new IntervalHeap<C5.KeyValuePair<Tile, int>>(new C5KeyValuePairComparer<Tile>());
			var explored = new int[grid.Length];
			var costSoFar = new int[grid.Length];
			var reachable = new List<ResultTile>();
			for (var i = 0; i < grid.Length; i++)
			{
				explored[i] = -1;
			}

			frontier.Add(new C5.KeyValuePair<Tile, int>(grid[startNode], -1));
			explored[startNode] = -1;
			costSoFar[startNode] = 0;
			reachable.Add(new ResultTile { Index = startNode, Cost = 0 });

			while (!frontier.IsEmpty)
			{
				Tile current = frontier.DeleteMin().Key;
				var currentIndex = current.IndexInGrid;

				var neighbors = grid[currentIndex].Neighbors;
				for (var i = 0; i < neighbors.Length; ++i)
				{
					Tile neighborTile = grid[neighbors[i]];
					var neighborIndex = neighborTile.IndexInGrid;
					var newCost = costSoFar[currentIndex] + grid.GetCost(currentIndex, neighborIndex);

					if ((explored[neighborIndex] == -1 || newCost < costSoFar[neighborIndex]) && isWalkable(neighborIndex) && newCost <= range)
					{
						costSoFar[neighborIndex] = newCost;
						frontier.Add(new C5.KeyValuePair<Tile, int>(neighborTile, newCost));
						explored[neighborIndex] = currentIndex;
						reachable.Add(new ResultTile { Index = neighborIndex, Cost = newCost });
					}
				}
			}

			return (reachable.ToArray(), explored);
		}

		/// <summary>
		///     Dijkstra implementation on indexes with walkable raycast check back to the start node.
		/// </summary>
		public static (int[], int[]) DijkstraRaycast
		(
			Grid1D<Tile> grid,
			int startNode,
			int range,
			Func<int, bool> isWalkable)
		{
			var frontier = new IntervalHeap<C5.KeyValuePair<Tile, int>>(new C5KeyValuePairComparer<Tile>());
			var explored = new int[grid.Length];
			var costSoFar = new int[grid.Length];
			var reachable = new List<int>();
			for (var i = 0; i < grid.Length; i++)
			{
				explored[i] = -1;
			}

			frontier.Add(new C5.KeyValuePair<Tile, int>(grid[startNode], -1));
			explored[startNode] = -1;
			costSoFar[startNode] = 0;

			while (!frontier.IsEmpty)
			{
				Tile current = frontier.DeleteMin().Key;
				var currentIndex = current.IndexInGrid;

				var neighbors = grid[currentIndex].Neighbors;
				for (var i = 0; i < neighbors.Length; ++i)
				{
					Tile neighborTile = grid[neighbors[i]];
					var neighborIndex = neighborTile.IndexInGrid;
					var newCost = costSoFar[currentIndex] + grid.GetCost(currentIndex, neighborIndex);

					if ((explored[neighborIndex] == -1 || newCost < costSoFar[neighborIndex]) && isWalkable(neighborIndex) && newCost <= range)
					{
						var halfTile = new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
						Vector2 from = grid.GetWorldPositionFromTileIndex(neighborIndex) + halfTile;
						Vector2 to = grid.GetWorldPositionFromTileIndex(startNode) + halfTile;
						if (grid.Raycast(from, to).Any(index => !isWalkable(index)))
						{
							continue;
						}

						costSoFar[neighborIndex] = newCost;
						frontier.Add(new C5.KeyValuePair<Tile, int>(neighborTile, newCost));
						explored[neighborIndex] = currentIndex;
						reachable.Add(neighborIndex);
					}
				}
			}

			return (reachable.ToArray(), explored);
		}

		public class ResultTile
		{
			public int Index;
			public int Cost;
		}
	}
}
