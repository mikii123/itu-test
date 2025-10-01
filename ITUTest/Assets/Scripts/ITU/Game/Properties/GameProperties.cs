using ITU.Algorithms;
using ITU.Grid;

namespace ITU.Game.Properties
{
	public static class GameProperties
	{
		public static GridProperties? GridProperties { get; set; }
		public static Grid1D<Tile> Grid { get; set; }
	}
}
