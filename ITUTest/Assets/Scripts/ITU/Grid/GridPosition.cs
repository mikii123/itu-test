using System;
using UnityEngine;

namespace ITU.Grid
{
	/// <summary>
	///     Represents a tile position in grid space.
	/// </summary>
	public struct GridPosition : IEquatable<GridPosition>
	{
		public bool Equals(GridPosition other)
		{
			return i == other.i && j == other.j;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			return obj is GridPosition position && Equals(position);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (i * 397) ^ j;
			}
		}

		/// <summary>
		///     column (x)
		/// </summary>
		public int i;

		/// <summary>
		///     row (y)
		/// </summary>
		public int j;

		public GridPosition(int i, int j)
		{
			this.i = i;
			this.j = j;
		}

		public GridPosition(Vector2 vec)
		{
			i = (int)vec.x;
			j = (int)vec.y;
		}

		public static bool operator==(GridPosition a, GridPosition b)
		{
			return a.i == b.i && a.j == b.j;
		}

		public static bool operator!=(GridPosition a, GridPosition b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return $"( {i} , {j} )";
		}

		public Vector2 Vector2 => new Vector2(i, j);
	}
}
