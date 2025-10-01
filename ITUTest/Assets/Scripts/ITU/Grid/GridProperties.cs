namespace ITU.Grid
{
	public struct GridProperties
	{
		public readonly int Columns;
		public readonly int Rows;
		public readonly long TileSize;

		public GridProperties(int columns, int rows, long tileSize)
		{
			Columns = columns;
			Rows = rows;
			TileSize = tileSize;
		}
	}
}
