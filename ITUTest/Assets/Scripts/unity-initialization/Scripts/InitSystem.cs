using System.Threading.Tasks;

namespace LELWare.Initialization
{
	public interface IInitSystem
	{
		Task Startup(params InitStep[] steps);
		Task Shutdown(params InitStep[] steps);
	}

	public class InitSystem : IInitSystem
	{
		public async Task Startup(params InitStep[] steps)
		{
			var group = new InitSequentialGroup(steps);
			await group.Startup();
		}

		public async Task Shutdown(params InitStep[] steps)
		{
			var group = new InitSequentialGroup(steps);
			await group.Shutdown();
		}
	}
}
