using ITU.Algorithms;
using ITU.Grid;
using ITU.Utilities;

namespace ITU.Game.Properties
{
	public static class GameProperties
	{
		public static GridProperties? GridProperties { get; set; }
		public static Grid1D<Tile> Grid { get; set; }
		public static Grid1D<TileView> Views { get; set; }

		public static ObservableProperty<int> MoveRange { get; set; } = new();
		public static ObservableProperty<int> AttackRange { get; set; } = new();
	}
}
