using System.Threading.Tasks;
using ITU.Algorithms;
using ITU.Game.Properties;
using ITU.Grid;
using LELWare.Initialization;

namespace ITU.Game.Init
{
	public class SetupGridInitStep : InitStep
	{
		public override Task Startup()
		{
			GameProperties.GridProperties ??= new GridProperties(20, 20, 1);
			GameProperties.MoveRange = new (5);
			GameProperties.AttackRange = new (1000);

			var grid = new Grid1D<Tile>(GameProperties.GridProperties.Value);
			GameProperties.Grid = grid;
			Parallel.For(
				0,
				GameProperties.Grid.Length,
				i =>
				{
					var tile = new Tile
					{
						IndexInGrid = i,
						Neighbors = grid.GetNeighborIndexesNoDiagonals(i)
					};
					grid[i] = tile;
				});

			return Task.CompletedTask;
		}

		public override Task Shutdown()
		{
			GameProperties.Grid = null;
			return Task.CompletedTask;
		}
	}
}
