using System.Threading.Tasks;

namespace LELWare.Initialization
{
	public class InitParallelGroup : InitStep
	{
		protected readonly InitStep[] _steps;

		public InitParallelGroup(params InitStep[] steps)
		{
			_steps = steps;
		}

		public override async Task Startup()
		{
			var tab = new Task[_steps.Length];
			for (var i = 0; i < _steps.Length; i++)
			{
				InitStep initStep = _steps[i];
				tab[i] = initStep.Startup();
			}

			await Task.WhenAll(tab);
		}

		public override async Task Shutdown()
		{
			var tab = new Task[_steps.Length];
			for (var i = _steps.Length - 1; i >= 0; i--)
			{
				InitStep initStep = _steps[i];
				tab[i] = initStep.Shutdown();
			}

			await Task.WhenAll(tab);
		}
	}
}
