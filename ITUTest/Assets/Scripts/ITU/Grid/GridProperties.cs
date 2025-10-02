namespace ITU.Grid
{
	public struct GridProperties
	{
		public readonly int Columns;
		public readonly int Rows;
		public readonly float TileSize;

		public GridProperties(int columns, int rows, float tileSize)
		{
			Columns = columns;
			Rows = rows;
			TileSize = tileSize;
		}
	}
}
