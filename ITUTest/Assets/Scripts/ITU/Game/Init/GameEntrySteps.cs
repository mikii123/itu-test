using LELWare.Initialization;

namespace ITU.Game.Init
{
	public class GameEntrySteps : InitSequentialGroup
	{
		public GameEntrySteps() : base(
			
			new InitParallelGroup(
				new PreloadAddressablesInitStep(),
				new SetupGridInitStep()
			),
			new SetupGridViewInitStep(),
			new SetupPlayerAndEnemyInitStep()
			
		)
		{ }
	}
}
