using System;
using Buildron.Domain.Mods;
using Buildron.ClassicMods.BuildMod.Controllers;
using Buildron.ClassicMods.BuildMod.Infrastructure;
using UnityEngine;
using Zenject;
using System.Linq;

namespace Buildron.ClassicMods.BuildMod
{
	public class Mod : IMod
	{
		public Mod ()
		{
		}

		public static IModContext Context { get; private set; }

		#region Methods
		public void Initialize (IModContext context)
		{
			Context = context;

			context.Log.Debug ("Creating controllers...");           
            var infra = context.Assets.Load("Infrastructure");
            var infraGO = context.GameObjects.Create(infra);

            var sceneContext = infraGO.GetComponentInChildren<SceneContext>();

            if (sceneContext == null)
            {
                throw new InvalidOperationException("SceneContext not found.");
            }


            var c = context.GameObjects.Create<BuildsDeployController>();
            context.Log.Debug ("Injecting: {0}", c);            
            sceneContext.Container.Inject(c);
		}
		#endregion
	}
}