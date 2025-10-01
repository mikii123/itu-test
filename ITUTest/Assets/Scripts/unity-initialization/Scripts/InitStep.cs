using System.Threading.Tasks;

namespace LELWare.Initialization
{
	public abstract class InitStep
	{
		public abstract Task Startup();
		public abstract Task Shutdown();
	}
}
