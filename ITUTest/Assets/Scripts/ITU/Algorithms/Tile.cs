using System;

namespace ITU.Algorithms
{
	public class Tile
	{
		public int IndexInGrid;
		public int[] Neighbors;
		public TileType Type;

		public event Action<TileType> OnTypeChange;

		public void SetType(TileType type)
		{
			if (Type != type)
			{
				Type = type;
				OnTypeChange?.Invoke(type);
			}
		}
	}

	public enum TileType
	{
		Walkable = 0,
		Obstacle = 1,
		Cover = 2
	}
}
