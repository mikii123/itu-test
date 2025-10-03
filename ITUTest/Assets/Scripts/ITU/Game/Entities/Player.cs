using System.Collections.Generic;
using ITU.Game.Properties;
using UnityEngine;

namespace ITU.Game.Entities
{
	public class Player : Entity<Player>
	{
		[SerializeField] private LineRenderer pathLine;
		[SerializeField] private LineRenderer shotLine;

		private List<int> _path;
		private bool _executing;

		private void Start()
		{
			pathLine.gameObject.SetActive(false);
			shotLine.gameObject.SetActive(false);
		}

		public void SetPath(List<int> path)
		{
			if (!_executing)
			{
				_path = path;
			}

			UpdateLine();
		}

		public void ExecutePath(List<int> path)
		{
			if (!_executing)
			{
				_path = path;
			}

			_executing = true;
			UpdateLine();
		}

		public void SetShot(int from, int to)
		{
			var grid = GameProperties.Grid;
			Vector2 fromV = grid.GetWorldPositionFromTileIndex(from) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
			Vector2 toV = grid.GetWorldPositionFromTileIndex(to) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);

			shotLine.gameObject.SetActive(true);
			shotLine.positionCount = 2;
			shotLine.SetPositions(new[] { new Vector3(fromV.x, 0, fromV.y), new Vector3(toV.x, 0, toV.y) });
		}

		public void SetShot(bool visible)
		{
			shotLine.gameObject.SetActive(false);
		}

		private void UpdateLine()
		{
			if (_path == null || _path.Count <= 0)
			{
				pathLine.gameObject.SetActive(false);
				return;
			}

			pathLine.gameObject.SetActive(true);

			var grid = GameProperties.Grid;
			pathLine.positionCount = _path.Count + 1;
			pathLine.SetPosition(0, new Vector3(transform.position.x, 0, transform.position.z));
			for (var index = 0; index < _path.Count; index++)
			{
				var gridIndex = _path[index];
				Vector2 vector2 = grid.GetWorldPositionFromTileIndex(gridIndex) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
				pathLine.SetPosition(index + 1, new Vector3(vector2.x, 0, vector2.y));
			}
		}
	}
}
