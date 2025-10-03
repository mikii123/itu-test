using System;
using ITU.Algorithms;
using ITU.Game.Properties;
using UnityEngine;

public class TileView : MonoBehaviour
{
	public Tile Tile { get; private set; }

	[SerializeField] private TileType _type;
	[SerializeField] private Highlight _highlightType;

	[SerializeField] private MeshRenderer main;
	[SerializeField] private MeshRenderer highlight;

	[SerializeField] private Material WalkableMat;
	[SerializeField] private Material ObstacleMat;
	[SerializeField] private Material CoverMat;

	[SerializeField] private Material HighlightMat;
	[SerializeField] private Material HighlightRedMat;
	[SerializeField] private Material HighlightBlueMat;
	
	private static Material _walkableMat;
	private static Material _obstacleMat;
	private static Material _coverMat;
	
	private static Material _highlightNoneMat;
	private static Material _highlightBlueMat;
	private static Material _highlightRedMat;

	public enum Highlight
	{
		None,
		Blue,
		Red
	}
	
	private void Awake()
	{
		_walkableMat ??= new Material(WalkableMat);
		_obstacleMat ??= new Material(ObstacleMat);
		_coverMat ??= new Material(CoverMat);

		_highlightNoneMat ??= new Material(HighlightMat);
		_highlightBlueMat ??= new Material(HighlightBlueMat);
		_highlightRedMat ??= new Material(HighlightRedMat);
	}

	private void OnDestroy()
	{
		if (Tile != null)
		{
			Tile.OnTypeChange -= OnTypeChanged;
		}
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

	public void SetHighlight(Highlight type)
	{
		_highlightType = type;
		switch (type)
		{
			case Highlight.None:
				highlight.sharedMaterial = _highlightNoneMat;
				break;
			case Highlight.Blue:
				highlight.sharedMaterial = _highlightBlueMat;
				break;
			case Highlight.Red:
				highlight.sharedMaterial = _highlightRedMat;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
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
}
