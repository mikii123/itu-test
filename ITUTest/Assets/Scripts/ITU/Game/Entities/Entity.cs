using ITU.Algorithms;
using ITU.Game.Properties;
using ITU.Utilities;
using UnityEngine;

namespace ITU.Game.Entities
{
	public class Entity<T> : Singleton<T>
		where T : Singleton<T>
	{
		public void SetPosition(Tile tile)
		{
			var grid = GameProperties.Grid;
			Vector2 vector2 = grid.GetWorldPositionFromTileIndex(tile.IndexInGrid) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
			var vector3 = new Vector3(vector2.x, 0, vector2.y);
			transform.position = vector3;
		}
	}
}
