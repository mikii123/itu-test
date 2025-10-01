using System.Threading.Tasks;

namespace LELWare.Initialization
{
	public class InitSequentialGroup : InitStep
	{
		protected readonly InitStep[] _steps;

		public InitSequentialGroup(params InitStep[] steps)
		{
			_steps = steps;
		}

		public override async Task Startup()
		{
			for (var i = 0; i < _steps.Length; i++)
			{
				InitStep initStep = _steps[i];
				await initStep.Startup();
			}
		}

		public override async Task Shutdown()
		{
			for (var i = _steps.Length - 1; i >= 0; i--)
			{
				InitStep initStep = _steps[i];
				await initStep.Shutdown();
			}
		}
	}
}
