using System;
using Buildron.Domain.Mods;
using Buildron.ClassicMods.BuildMod.Controllers;
using Buildron.ClassicMods.BuildMod.Infrastructure;
using UnityEngine;
using Zenject;
using System.Linq;
using Buildron.Domain.RemoteControls;

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
            RegisterPreferences();
            CreateControllers();
        }

        private void RegisterPreferences()
        {
            Context.Preference.Register(
                new Preference("BuildsTotemsNumber", "Totems", PreferenceKind.String, 1),
                new Preference("HistoryTotemEnabled", "History", PreferenceKind.Bool, true));
        }

        private void CreateControllers()
        {
            Context.Log.Debug("Creating controllers...");
            var infra = Context.Assets.Load("Infrastructure");
            var infraGO = Context.GameObjects.Create(infra);

            var sceneContext = infraGO.GetComponentInChildren<SceneContext>();

            if (sceneContext == null)
            {
                throw new InvalidOperationException("SceneContext not found.");
            }

            DI = sceneContext;

            CreateController("BuildsDeployController");
            CreateController("BuildHistoryController");
            CreateController("BuildsController");

            BuildsHistoryCameraController cameraController = null;

            if (Context.Preference.GetValue<bool>("HistoryTotemEnabled"))
            {
                Context.RemoteControlCommandReceived += (sender, e) =>
                {
                    var cmd = e.Command as CustomRemoteControlCommand;

                    if (cmd != null)
                    {
                        switch (cmd.Name)
                        {
                            case "ShowHistory":
                                cameraController = Context.Camera.RegisterController<BuildsHistoryCameraController>(CameraControllerKind.Position, true);
                                cameraController.ShowHistory();
                                break;

                            case "ShowBuilds":
                                cameraController.ShowBuilds();
                                break;
                        }
                    }
                };
            }
        }

        private void CreateController (string prefabName)
		{
			var prefab = Context.Assets.Load (prefabName);
			var go = Context.GameObjects.Create (prefab);
			DI.Container.InjectGameObject (go, true);
		}

		private void CreateController<TController> ()
            where TController : MonoBehaviour
		{
			Context.Log.Debug ("Creating controller: {0}", typeof(TController).Name);
			var c = Context.GameObjects.Create<TController> ();            
			DI.Container.Inject (c);
		}
		#endregion
	}
}