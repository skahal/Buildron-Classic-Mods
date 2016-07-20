using System;
using Zenject;
using Buildron.ClassicMods.BuildMod.Application;
using Buildron.ClassicMods.BuildMod.Controllers;
using Skahal.Logging;

namespace Buildron.ClassicMods.BuildMod.Infrastructure
{
	public class IoCInstaller : MonoInstaller
	{
		public override void InstallBindings ()
		{	
			// TODO: this should be installed by emulator
			Container.Bind<ISHLogStrategy>().To<SHDebugLogStrategy>().AsSingle();
			//

			Container.Bind<BuildGOService> ().AsSingle ();
			//Container.BindFactory<BuildController, BuildController.Factory> ()
			//	.FromPrefabResource ("BuildPrefab")
			//	.UnderGameObjectGroup ("Builds");            
		}
	}
}

