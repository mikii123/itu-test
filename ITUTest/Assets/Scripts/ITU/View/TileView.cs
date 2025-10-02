using System;
using ITU.Algorithms;
using ITU.Game.Properties;
using UnityEngine;

public class TileView : MonoBehaviour
{
	public Tile Tile { get; private set; }

	[SerializeField] private TileType _type;
	
	[SerializeField] private MeshRenderer main;
	[SerializeField] private MeshRenderer highlight;
	
	[SerializeField] private Material WalkableMat;
	private static Material _walkableMat;
	[SerializeField] private Material ObstacleMat;
	private static Material _obstacleMat;
	[SerializeField] private Material CoverMat;
	private static Material _coverMat;

	[SerializeField] private Material HighlightMat;
	[SerializeField] private Material HighlightRedMat;
	[SerializeField] private Material HighlightBlueMat;

	private void Awake()
	{
		_walkableMat ??= new Material(WalkableMat);
		_obstacleMat ??= new Material(ObstacleMat);
		_coverMat ??= new Material(CoverMat);
	}

	public void Setup(Tile tile)
	{
		Tile = tile;

		tile.OnTypeChange += OnTypeChanged;
		
		transform.localScale = Vector3.one * GameProperties.GridProperties.Value.TileSize;
		Vector2 vector2 = GameProperties.Grid.GetWorldPositionFromTileIndex(tile.IndexInGrid);
		transform.position = new Vector3(vector2.x, 0, vector2.y);
		
		OnTypeChanged(tile.Type);
	}

	private void OnTypeChanged(TileType type)
	{
		_type = type;
		switch (type)
		{
			case TileType.Walkable:
				main.sharedMaterial = _walkableMat;
				break;
			case TileType.Obstacle:
				main.sharedMaterial = _obstacleMat;
				break;
			case TileType.Cover:
				main.sharedMaterial = _coverMat;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnDestroy()
	{
		if (Tile != null)
		{
			Tile.OnTypeChange -= OnTypeChanged;
		}
	}
}
