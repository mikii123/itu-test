using System;
using System.Collections.Generic;
using ITU.Algorithms;
using ITU.Game.Properties;
using ITU.Grid;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ITU.Game.Entities
{
	public class Player : Entity<Player>
	{
		public event Action OnFinishExecution;

		[SerializeField] private LineRenderer pathLine;
		[SerializeField] private LineRenderer shotLine;
		[SerializeField] private Animator animator;
		
		private static readonly int RunParameter = Animator.StringToHash("Run");

		private List<int> _path;
		private (int from, int to)? _shot;
		private bool _executing;

		private void Start()
		{
			pathLine.gameObject.SetActive(false);
			shotLine.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (!_executing)
			{
				return;
			}

			CurrentState.Execute(this);
		}

		public void SetPath(List<int> path)
		{
			if (_executing)
			{
				return;
			}

			_path = path;
			UpdateLine();
		}

		public void SetShot((int from, int to)? shot)
		{
			if (_executing)
			{
				return;
			}

			_shot = shot;
			UpdateShot();
		}

		public bool ExecutePath()
		{
			if (_executing)
			{
				return false;
			}

			_executing = true;

			CurrentState = new MoveState();

			UpdateLine();
			UpdateShot();

			return true;
		}

		public void FinishExecution()
		{
			SetPath(null);
			SetShot(null);

			_executing = false;

			UpdateLine();
			UpdateShot();

			OnFinishExecution?.Invoke();
		}

		private void UpdateShot()
		{
			if (_shot == null)
			{
				shotLine.gameObject.SetActive(false);
				return;
			}

			var grid = GameProperties.Grid;
			Vector2 fromV = grid.GetWorldPositionFromTileIndex(_shot.Value.from) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);
			Vector2 toV = grid.GetWorldPositionFromTileIndex(_shot.Value.to) + new Vector2(grid.Properties.TileSize / 2, grid.Properties.TileSize / 2);

			shotLine.gameObject.SetActive(true);
			shotLine.positionCount = 2;
			shotLine.SetPositions(new[] { new Vector3(fromV.x, 0, fromV.y), new Vector3(toV.x, 0, toV.y) });
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

		private class MoveState : State<Player>
		{
			private Grid1D<Tile> _grid;
			private float _time;
			private Vector3 _startPos;
			private Vector3 _targetPos;

			protected override void OnInit(Player entity)
			{
				if (entity._path == null || entity._path.Count == 0)
				{
					entity.CurrentState = new ShootState();
					return;
				}

				_grid = GameProperties.Grid;
				_startPos = entity.transform.position;

				Vector2 position = _grid.GetWorldPositionFromTileIndex(entity._path[0]) + new Vector2(_grid.Properties.TileSize / 2, _grid.Properties.TileSize / 2);
				_targetPos = new Vector3(position.x, 0, position.y);
				entity.transform.rotation = Quaternion.LookRotation(_targetPos - _startPos);
			}

			protected override void OnExecute(Player entity)
			{
				if (entity._path.Count == 0)
				{
					entity.CurrentState = new ShootState();
					return;
				}

				if (entity._path.Count > 0)
				{
					entity.animator.SetBool(RunParameter, true);

					if (_time <= 1)
					{
						entity.transform.position = Vector3.Lerp(_startPos, _targetPos, _time);

						_time += Time.deltaTime / 0.5f;

						if (_time > 1)
						{
							_time = 0;
							entity._path.RemoveAt(0);

							if (entity._path.Count > 0)
							{
								Vector2 position = _grid.GetWorldPositionFromTileIndex(entity._path[0]) + new Vector2(_grid.Properties.TileSize / 2, _grid.Properties.TileSize / 2);
								_startPos = entity.transform.position;
								_targetPos = new Vector3(position.x, 0, position.y);

								entity.transform.rotation = Quaternion.LookRotation(_targetPos - _startPos);
							}
						}
					}
				}
				else
				{
					entity.CurrentState = new ShootState();
				}
			}
		}

		private class ShootState : State<Player>
		{
			private Grid1D<Tile> _grid;
			private Vector3 _startPos;
			private Vector3 _targetPos;
			private float _time;
			private float _distance;

			private GameObject shot;

			protected override async void OnInit(Player entity)
			{
				if (entity._shot == null)
				{
					entity.CurrentState = new ExitState();
					return;
				}

				_grid = GameProperties.Grid;
				Vector2 vector2 = _grid.GetWorldPositionFromTileIndex(entity._shot.Value.from) + new Vector2(_grid.Properties.TileSize / 2, _grid.Properties.TileSize / 2);
				Vector2 vector2To = _grid.GetWorldPositionFromTileIndex(entity._shot.Value.to) + new Vector2(_grid.Properties.TileSize / 2, _grid.Properties.TileSize / 2);
				_startPos = new Vector3(vector2.x, 0.5f, vector2.y);
				_targetPos = new Vector3(vector2To.x, 0.5f, vector2To.y);
				_distance = Vector3.Distance(_targetPos, _startPos);

				var handle = Addressables.InstantiateAsync("Shot", _startPos, Quaternion.LookRotation(_targetPos - _startPos));
				await handle.Task;
				shot = handle.Result;

				entity.transform.rotation = Quaternion.LookRotation(_targetPos - _startPos);

				entity.animator.SetBool(RunParameter, false);
			}

			protected override void OnExecute(Player entity)
			{
				if (shot == null)
				{
					return;
				}

				if (_time <= 1)
				{
					shot.transform.position = Vector3.Lerp(_startPos, _targetPos, _time);

					_time += Time.deltaTime / _distance * 30;

					if (_time > 1)
					{
						Addressables.InstantiateAsync("Explosion", _targetPos, Quaternion.identity);
						Destroy(shot);
						entity.CurrentState = new ExitState();
					}
				}
			}
		}

		private class ExitState : State<Player>
		{
			protected override void OnInit(Player entity)
			{
				entity.animator.SetBool(RunParameter, false);
				entity.FinishExecution();
			}

			protected override void OnExecute(Player entity)
			{ }
		}
	}
}
