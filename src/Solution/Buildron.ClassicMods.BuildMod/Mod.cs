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
        public static SceneContext DI { get; private set; }


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

            DI = sceneContext;

            CreateController("BuildsDeployController");            
            CreateController("BuildHistoryController"); 
			CreateController("BuildsController"); 
        }

        private void CreateController(string prefabName)
        {
            var prefab = Context.Assets.Load(prefabName);
            var go = Context.GameObjects.Create(prefab);
            DI.Container.InjectGameObject(go, true);
        }

        private void CreateController<TController>()
            where TController : MonoBehaviour
        {
            Context.Log.Debug("Creating controller: {0}", typeof(TController).Name);
            var c = Context.GameObjects.Create<TController>();            
            DI.Container.Inject(c);
        }
		#endregion
	}
}