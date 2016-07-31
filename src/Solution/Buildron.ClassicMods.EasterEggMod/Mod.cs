using System;
using Buildron.Domain.Mods;
using Buildron.Domain.RemoteControls;
using Buildron.Domain.EasterEggs;

namespace Buildron.ClassicMods.EasterEggMod
{
	public class Mod : IMod
	{
		public static IModContext Context { get; private set; }

		public void Initialize (IModContext context)
		{
			Context = context;
			var matrixController = context.GameObjects.Create<MatrixEasterEggController> ();

			var service = new EasterEggService (
				              new IEasterEggProvider[] { matrixController }, 
				              context.Log);
			
			context.RemoteControlCommandReceived += (sender, e) => {
				var filterCmd = e.Command as FilterBuildsRemoteControlCommand;

				if(filterCmd != null && service.IsEasterEggMessage(filterCmd.KeyWord)) {
					e.Cancel = true;
					service.ReceiveEasterEgg(filterCmd.KeyWord);
				}
			};
		}
	}
}