using Zenject;
using Buildron.ClassicMods.BuildMod.Application;

namespace Buildron.ClassicMods.BuildMod.Infrastructure
{
    public class IoCInstaller : MonoInstaller
	{
		public override void InstallBindings ()
		{	
			Container.Bind<BuildGOService> ().AsSingle ();
		}
	}
}

