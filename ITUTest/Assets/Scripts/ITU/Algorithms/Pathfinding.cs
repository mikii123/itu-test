using System;
using System.Collections.Generic;
using C5;
using ITU.Grid;
using Unity.VisualScripting;

namespace ITU.Algorithms
{
	public static class Pathfinding
	{
		/// <summary>
		///     Dijkstra implementation on short table indexes.
		/// </summary>
		public static int GetWalkable
		(
			Grid1D<Tile> grid,
			int startNode,
			int range,
			Func<int, bool> isWalkable
		)
		{
			int goal = -1;
			var frontier = new Queue<Tile>();
			var explored = new short[grid.Length];
			var costSoFar = new int[grid.Length];
			for (var i = 0; i < grid.Length; i++)
			{
				explored[i] = -1;
			}

			frontier.Enqueue(grid[startNode]);
			explored[startNode] = (short)startNode;
			costSoFar[startNode] = 0;

			while (frontier.Count > 0)
			{
				Tile current = frontier.Dequeue();
				int currentIndex = current.IndexInGrid;

				if (isWalkable(currentIndex))
				{
					goal = currentIndex;
					break;
				}

				for (var i = 0; i < grid[currentIndex].Neighbors.Length; ++i)
				{
					int newCost = costSoFar[currentIndex] + 1;
					Tile neighborTile = grid[grid[currentIndex].Neighbors[i]];
					int neighborIndex = neighborTile.IndexInGrid;

					if ((explored[neighborIndex] == -1 || newCost < costSoFar[neighborIndex]) && newCost < range)
					{
						costSoFar[neighborIndex] = newCost;
						frontier.Enqueue(neighborTile);
						explored[neighborIndex] = (short)current.IndexInGrid;
					}
				}
			}

			return goal;
		}

		/// <summary>
		///     Dijkstra implementation on indexes.
		/// </summary>
		public static (int[], int[]) Dijkstra
		(
			ref Grid1D<Tile> grid,
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
				int currentIndex = current.IndexInGrid;

				var neighbors = grid[currentIndex].Neighbors;
				for (var i = 0; i < neighbors.Length; ++i)
				{
					Tile neighborTile = grid[neighbors[i]];
					int neighborIndex = neighborTile.IndexInGrid;
					int newCost = costSoFar[currentIndex] + grid.GetCost(currentIndex, neighborIndex);

					if ((explored[neighborIndex] == -1 || newCost < costSoFar[neighborIndex]) && isWalkable(neighborIndex) && newCost < range)
					{
						costSoFar[neighborIndex] = newCost;
						frontier.Add(new C5.KeyValuePair<Tile, int>(neighborTile, newCost));
						explored[neighborIndex] = currentIndex;
						reachable.Add(neighborIndex);
					}
				}
			}

			return (reachable.ToArray(), explored);
		}
	}
}
