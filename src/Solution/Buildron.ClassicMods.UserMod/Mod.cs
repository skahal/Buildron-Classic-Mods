using System;
using Buildron.Domain.Mods;

namespace Buildron.ClassicMods.UserMod
{
	public class Mod : IMod
	{
		public static IModContext Context { get; private set; }

		public void Initialize (IModContext context)
		{
			Context = context;

			context.CIServerConnected += (sender, e) => 
			{
				context.CreateGameObjectFromPrefab("UsersManagerPrefab");
			};
		}
	}
}
