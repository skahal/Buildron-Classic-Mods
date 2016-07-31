using System;
using Buildron.Domain.Mods;
using Buildron.Domain.RemoteControls;
using Buildron.Domain.EasterEggs;

namespace Buildron.ClassicMods.EasterEggMod
{
	public class Mod : IMod
	{
		public static IModContext Context { get; private set; }
		public static IRemoteControl ConnectedRC { get; private set; }

		public void Initialize (IModContext context)
		{
			Context = context;
			context.GameObjectsPool.CreatePool ("Ball", () => {
				return context.CreateGameObjectFromPrefab("BallPrefab");
			});

			var matrixController = context.GameObjects.Create<MatrixEasterEggController> ();
			var kickController = context.GameObjects.Create<KickEasterEggController> ();

			var service = new EasterEggService (
				              new IEasterEggProvider[] { matrixController, kickController }, 
				              context.Log);
			
			context.RemoteControlCommandReceived += (sender, e) => {
				var filterCmd = e.Command as FilterBuildsRemoteControlCommand;

				if (filterCmd != null && service.IsEasterEggMessage (filterCmd.KeyWord)) {
					e.Cancel = true;
					ConnectedRC = e.RemoteControl;
					service.ReceiveEasterEgg (filterCmd.KeyWord);
				}
			};
		}
	}
}