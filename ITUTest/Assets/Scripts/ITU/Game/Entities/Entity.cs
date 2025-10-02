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
			Vector2 vector2 = GameProperties.Grid.GetWorldPositionFromTileIndex(tile.IndexInGrid);
			var vector3 = new Vector3(vector2.x, 0, vector2.y);
			transform.position = vector3;
		}
	}
}
