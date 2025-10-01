using System;

namespace ITU.Algorithms
{
	[Serializable]
	public struct Tile
	{
		public int IndexInGrid;
		public int[] Neighbors;
		public TileType Type;
	}

	public enum TileType
	{
		Walkable = 0,
		Obstacle = 1,
		Cover = 2
	}
}
