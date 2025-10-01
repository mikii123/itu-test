using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITU.Grid
{
	//////////////////////////////////////
	//                                  //
	// (jN, 0)                 (jN, iN) //
	//        o . . . . . . . o         //
	//        . . . . . . . . .         //
	//     r  . . . . . . . . .         //
	//     o  . . . . . . . . .         //
	//     w  . . . . . . . . .         //
	//     s  . . . . . . . . .         //
	//        . . . . . . . . .         //
	//        o . . . . . . . o         //
	//  (0, 0)     columns     (0, iN)  //
	//                                  //
	//////////////////////////////////////

	/// <summary>
	///     Represents a flattened 2D grid.
	/// </summary>
	public class Grid1D<T> : IEnumerable<T>
	{
		public T this[int i, int j]
		{
			get => m_grid[GetTileIndex(i, j)];
			set => m_grid[GetTileIndex(i, j)] = value;
		}

		public T this[float i, float y]
		{
			get => m_grid[GetTileIndexFromWorldPosition(i, y)];
			set => m_grid[GetTileIndexFromWorldPosition(i, y)] = value;
		}

		public T this[int i]
		{
			get => m_grid[i];
			set => m_grid[i] = value;
		}

		public T this[GridPosition gridPosition]
		{
			get => this[gridPosition.i, gridPosition.j];
			set => this[gridPosition.i, gridPosition.j] = value;
		}

		public GridProperties Properties { get; }
		public int Length => m_grid.Length;
		public T[] RawData => m_grid;

		protected readonly T[] m_grid;

		public Grid1D(GridProperties properties)
		{
			Properties = properties;
			m_grid = new T[Properties.Rows * Properties.Columns];
		}

		public Grid1D(T[] rawData, GridProperties properties)
		{
			Properties = properties;
			m_grid = rawData;
		}

		public int[] GetTilesInSquareArea(float x, float y, float extent)
		{
			var list = new List<int>();

			GridPosition upperLeft = GetTilePositionClamped(x - extent, y + extent);
			GridPosition downRight = GetTilePositionClamped(x + extent, y - extent);

			for (int i = upperLeft.i; i <= downRight.i; i++)
			{
				for (int j = downRight.j; j <= upperLeft.j; j++)
				{
					int tileIndex = GetTileIndex(i, j, Properties.Columns);
					if (tileIndex < Length)
					{
						list.Add(tileIndex);
					}
				}
			}

			return list.ToArray();
		}

		public static Vector2 GetTileWorldPosition(int tileIndex, GridProperties gridProperties)
		{
			GridPosition gridPos = GetGridPosFromIndex(tileIndex, gridProperties);
			var pos = new Vector2((float)gridPos.i * gridProperties.TileSize, (float)gridPos.j * gridProperties.TileSize);
			return pos;
		}

		public int[] GetTilesInArea(float x, float y, Vector2 extents)
		{
			var list = new List<int>();

			GridPosition upperLeft = GetTilePositionClamped(x - extents.x, y + extents.y);
			GridPosition downRight = GetTilePositionClamped(x + extents.x, y - extents.y);

			for (int i = upperLeft.i; i <= downRight.i; i++)
			{
				for (int j = downRight.j; j <= upperLeft.j; j++)
				{
					int tileIndex = GetTileIndex(i, j, Properties.Columns);
					if (tileIndex < Length)
					{
						list.Add(tileIndex);
					}
				}
			}

			return list.ToArray();
		}

		public static uint[] GetTilesInSquareArea(float x, float y, float extent, GridProperties gridProperties)
		{
			var list = new List<uint>();

			GridPosition upperLeft = GetTilePositionClamped(x - extent, y + extent, gridProperties);
			GridPosition downRight = GetTilePositionClamped(x + extent, y - extent, gridProperties);

			for (int i = upperLeft.i; i <= downRight.i; i++)
			{
				for (int j = downRight.j; j <= upperLeft.j; j++)
				{
					int tileIndex = GetTileIndex(i, j, gridProperties.Columns);
					if (tileIndex < gridProperties.Columns * gridProperties.Rows)
					{
						list.Add((uint)tileIndex);
					}
				}
			}

			return list.ToArray();
		}

		/// <summary>
		///     Get a tile at its index position
		/// </summary>
		public T GetTile(GridPosition pos)
		{
			int i = GetTileIndex(pos.i, pos.j);
			return m_grid[i];
		}

		/// <summary>
		///     Get a tile at world position
		/// </summary>
		public T GetTile(float x, float y)
		{
			int i = GetTileIndexFromWorldPosition(x, y);
			return m_grid[i];
		}

		public void ForEach(Func<T> a)
		{
			for (var i = 0; i < m_grid.Length; i++)
			{
				m_grid[i] = a();
			}
		}

		public void SetTile(uint index, T tile)
		{
			m_grid[index] = tile;
		}

		public void SetTile(GridPosition pos, T tile)
		{
			int index = GetTileIndex(pos);
			m_grid[index] = tile;
		}

		public void ForEach(Action<T> a)
		{
			for (var i = 0; i < m_grid.Length; i++)
			{
				a(m_grid[i]);
			}
		}

		public static int GetTileIndex(int i, int j, int colums)
		{
			return j * colums + i;
		}

		public static GridPosition GetTilePosition(float x, float y, float tileSize)
		{
			float i = Mathf.Floor(x / tileSize);
			float j = Mathf.Floor(y / tileSize);

			return new GridPosition((int)i, (int)j);
		}

		public GridPosition[] GetNeighbors(GridPosition gridPosition)
		{
			var neighbors = new List<GridPosition>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
					{
						continue;
					}

					int xToCheck = gridPosition.i + x;
					int yToCheck = gridPosition.j + y;

					if (xToCheck >= 0
					    && xToCheck < Properties.Columns
					    && yToCheck >= 0
					    && yToCheck < Properties.Rows)
					{
						neighbors.Add(new GridPosition(xToCheck, yToCheck));
					}
				}
			}

			return neighbors.ToArray();
		}

		public int[] GetNeighborIndexes(GridPosition gridPosition)
		{
			var neighbors = new List<int>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
					{
						continue;
					}

					int xToCheck = gridPosition.i + x;
					int yToCheck = gridPosition.j + y;

					if (xToCheck >= 0
					    && xToCheck < Properties.Columns
					    && yToCheck >= 0
					    && yToCheck < Properties.Rows)
					{
						neighbors.Add(GetTileIndex(xToCheck, yToCheck));
					}
				}
			}

			return neighbors.ToArray();
		}

		public int[] GetNeighborIndexesNoDiagonals(GridPosition gridPosition)
		{
			var neighbors = new List<int>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if ((x == 0 && y == 0) ||
					    (x == 1 && y == 1) ||
					    (x == -1 && y == 1) ||
					    (x == -1 && y == -1) ||
					    (x == 1 && y == -1))
					{
						continue;
					}

					int xToCheck = gridPosition.i + x;
					int yToCheck = gridPosition.j + y;

					if (xToCheck >= 0
					    && xToCheck < Properties.Columns
					    && yToCheck >= 0
					    && yToCheck < Properties.Rows)
					{
						neighbors.Add(GetTileIndex(xToCheck, yToCheck));
					}
				}
			}

			return neighbors.ToArray();
		}

		public int[] GetNeighborIndexes(int index)
		{
			return GetNeighborIndexes(GetTilePosition(index));
		}

		public int[] GetNeighborIndexesNoDiagonals(int index)
		{
			return GetNeighborIndexesNoDiagonals(GetTilePosition(index));
		}

		/// <summary>
		///     Get general cost between neighboring tiles. 14 for diagonal, 10 for vertical and horizontal
		/// </summary>
		public int GetCost(int from, int to)
		{
			GridPosition start = GetTilePosition(from);
			GridPosition end = GetTilePosition(to);

			int cost = Math.Abs(start.i - end.i) + Math.Abs(start.j - end.j);
			if (cost > 1)
			{
				// diagonal
				cost = 14;
			}
			else
			{
				// horizontal / vertical
				cost = 10;
			}

			return cost;
		}

		public int GetTileIndex(int column, int row)
		{
			return row * Properties.Columns + column;
		}

		public int GetTileIndex(GridPosition pos)
		{
			return pos.j * Properties.Columns + pos.i;
		}

		public static GridPosition GetTilePosition(int i, int columns)
		{
			return new GridPosition
			{
				i = i % columns,
				j = i / columns
			};
		}

		/// <summary>
		///     Get a tile index at world position
		/// </summary>
		public int GetTileIndexFromWorldPosition(float x, float y)
		{
			GridPosition gridPosition = GetTilePositionFromWorldPosition(x, y);
			return GetTileIndex(gridPosition.i, gridPosition.j);
		}

		/// <summary>
		///     Get a tile index at world position
		/// </summary>
		public int GetTileIndexFromWorldPosition(Vector2 pos)
		{
			return GetTileIndexFromWorldPosition(pos.x, pos.y);
		}

		/// <summary>
		///     Get a tile index at world position
		/// </summary>
		public int GetTileIndexClampedFromWorldPosition(Vector2 pos)
		{
			GridPosition gridPosition = GetTilePositionClamped(pos);
			return GetTileIndex(gridPosition.i, gridPosition.j);
		}

		public Vector2 GetWorldPositionFromTileIndex(int index)
		{
			GridPosition gridPosition = GetTilePosition(index);
			return new Vector2(gridPosition.i, gridPosition.j) * Properties.TileSize;
		}

		public Vector2 GetWorldPositionFromGridPosition(GridPosition position)
		{
			return new Vector2(position.i, position.j) * Properties.TileSize;
		}

		public GridPosition GetTilePosition(int index)
		{
			if (index > m_grid.Length - 1)
			{
				throw new ArgumentOutOfRangeException($"Index: {index}");
			}

			return new GridPosition
			{
				i = index % Properties.Columns,
				j = index / Properties.Columns
			};
		}

		public GridPosition GetTilePositionFromWorldPosition(float x, float y)
		{
			var i = (int)Mathf.Floor(x / Properties.TileSize);
			var j = (int)Mathf.Floor(y / Properties.TileSize);

			if (!IsPositionInsideGrid(i, j))
			{
				throw new ArgumentOutOfRangeException($"i = {i}; j = {j}");
			}

			return new GridPosition(i, j);
		}

		public static int GetIndexFromGridPos(GridPosition pos, GridProperties gridProperties)
		{
			return GetTileIndex(pos.i, pos.j, gridProperties.Columns);
		}

		public static GridPosition GetGridPosFromIndex(int index, GridProperties gridProperties)
		{
			return new GridPosition
			{
				i = index % gridProperties.Columns,
				j = index / gridProperties.Columns
			};
		}

		public GridPosition GetTilePositionFromWorldPosition(Vector2 position)
		{
			return GetTilePositionFromWorldPosition(position.x, position.y);
		}

		/// <returns>If the position is outside the grid it will return a Grid1D.Position closest to that point</returns>
		public GridPosition GetTilePositionClamped(float x, float y)
		{
			var i = (int)Mathf.Floor(x / Properties.TileSize);
			var j = (int)Mathf.Floor(y / Properties.TileSize);

			if (!IsPositionInsideGrid(i, j))
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i > Properties.Columns - 1)
				{
					i = Properties.Columns - 1;
				}

				if (j < 0)
				{
					j = 0;
				}
				else if (j > Properties.Rows - 1)
				{
					j = Properties.Rows - 1;
				}
			}

			return new GridPosition(i, j);
		}

		public GridPosition GetTilePositionClamped(Vector2 position)
		{
			return GetTilePositionClamped(position.x, position.y);
		}

		/// <returns>If the position is outside the grid it will return a Grid1D.Position closest to that point</returns>
		public static GridPosition GetTilePositionClamped(float x, float y, GridProperties properties)
		{
			var i = (int)Mathf.Floor(x / properties.TileSize);
			var j = (int)Mathf.Floor(y / properties.TileSize);

			if (!IsPositionInsideGrid(i, j, properties.Columns, properties.Rows))
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i > properties.Columns - 1)
				{
					i = properties.Columns - 1;
				}

				if (j < 0)
				{
					j = 0;
				}
				else if (j > properties.Rows - 1)
				{
					j = properties.Rows - 1;
				}
			}

			return new GridPosition(i, j);
		}

		public bool IsPositionInsideGrid(GridPosition pos)
		{
			return IsPositionInsideGrid(pos.i, pos.j);
		}

		public bool IsPositionInsideGrid(int x, int y)
		{
			return !(x < 0 || y < 0 || x > Properties.Columns - 1 || y > Properties.Rows - 1);
		}

		public static bool IsPositionInsideGrid(int x, int y, int columns, int rows)
		{
			return !(x < 0 || y < 0 || x > columns - 1 || y > rows - 1);
		}

		public bool IsWorldPositionInsideGrid(float x, float y)
		{
			return !(x < 0 || y < 0 || x > (float)Properties.Columns * Properties.TileSize || y > (float)Properties.Rows * Properties.TileSize);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Grid1DEnumerator<T>(m_grid);
		}

		public class Grid1DEnumerator<E> : IEnumerator<E>
		{
			public E Current
			{
				get => reference[index];
				set => reference[index] = value;
			}

			object IEnumerator.Current => Current;
			private int index;
			private E[] reference;

			public Grid1DEnumerator(E[] grid)
			{
				index = -1;
				reference = grid;
			}

			public void Dispose()
			{
				reference = null;
			}

			public void Reset()
			{
				index = -1;
			}

			public bool MoveNext()
			{
				index++;

				return index <= reference.Length - 1;
			}
		}
	}
}
