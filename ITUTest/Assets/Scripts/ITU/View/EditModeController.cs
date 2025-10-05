using ITU.Algorithms;
using ITU.Game.Entities;
using ITU.Game.Init;
using ITU.Game.Properties;
using ITU.Grid;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITU.View
{
	public class EditModeController : MonoBehaviour
	{
		public enum BrushType
		{
			None = -1,
			Walkable = 0,
			Obstacle = 1,
			Cover = 2,
			Player = 3,
			Enemy = 4
		}

		[SerializeField] private GameEntry game;
		[SerializeField] private LayerMask brushMask;

		[SerializeField] private TMP_InputField moveRange;
		[SerializeField] private TMP_InputField attackRange;
		[SerializeField] private TMP_InputField columns;
		[SerializeField] private TMP_InputField rows;
		[SerializeField] private TMP_InputField tileSize;

		private BrushType _currentBrush = BrushType.None;

		private void Start()
		{
			moveRange.onValueChanged.AddListener(MoveRangeOnValueChanged);
			attackRange.onValueChanged.AddListener(AttackRangeOnValueChanged);
			columns.onValueChanged.AddListener(ColumnsOnChange);
			rows.onValueChanged.AddListener(RowsOnChange);
			tileSize.onValueChanged.AddListener(TileSizeOnChange);

			moveRange.text = GameProperties.MoveRange.Value.ToString();
			attackRange.text = GameProperties.AttackRange.Value.ToString();
			columns.text = GameProperties.GridProperties.Value.Columns.ToString();
			rows.text = GameProperties.GridProperties.Value.Rows.ToString();
			tileSize.text = GameProperties.GridProperties.Value.TileSize.ToString();
		}

		private void Update()
		{
			if (!Input.GetMouseButton(0) || _currentBrush == BrushType.None)
			{
				return;
			}

			Camera cam = CameraController.Instance.Camera;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit info, 1000, brushMask))
			{
				var tileView = info.transform.GetComponentInParent<TileView>();
				if (tileView != null)
				{
					switch (_currentBrush)
					{
						case BrushType.Walkable or BrushType.Obstacle or BrushType.Cover:
							tileView.Tile.SetType((TileType)_currentBrush);
							break;
						case BrushType.Player:
							if (tileView.Tile.Type == TileType.Walkable)
							{
								Player.Instance.SetPosition(tileView.Tile);
							} 
							break;
						case BrushType.Enemy:
							if (tileView.Tile.Type == TileType.Walkable)
							{
								Enemy.Instance.SetPosition(tileView.Tile);
							} 
							break;
					}
				}
			}
		}

		public void OnSwitchBrush(BrushType type)
		{
			_currentBrush = type;
		}

		public async void OnReload()
		{
			await game.Reload();
		}

		private void ColumnsOnChange(string value)
		{
			var properties = GameProperties.GridProperties.Value;
			GameProperties.GridProperties = new GridProperties(int.Parse(value), properties.Rows, properties.TileSize);
		}

		private void RowsOnChange(string value)
		{
			var properties = GameProperties.GridProperties.Value;
			GameProperties.GridProperties = new GridProperties(properties.Columns, int.Parse(value), properties.TileSize);
		}

		private void TileSizeOnChange(string value)
		{
			var properties = GameProperties.GridProperties.Value;
			GameProperties.GridProperties = new GridProperties(properties.Columns, properties.Rows, float.Parse(value));
		}

		private void MoveRangeOnValueChanged(string value)
		{
			GameProperties.MoveRange.Set(int.Parse(value));
		}

		private void AttackRangeOnValueChanged(string value)
		{
			GameProperties.AttackRange.Set(int.Parse(value));
		}
	}
}
