using System;
using Buildron.Domain.Mods;
using UnityEngine;

namespace Buildron.ClassicMods.CameraMod
{
    public class Mod : IMod
    {
        private CameraController m_controller;

        public static IModContext Context { get; private set; }

        public void Initialize(IModContext context)
        {
            Context = context;

            context.Preferences.Register(
                new Preference("AutoPosition", "Auto position", PreferenceKind.Bool, true));

            context.CIServerConnected += delegate
            {
                m_controller = context.Camera.RegisterController<CameraController>(CameraControllerKind.Position, true);

                if (context.Preferences.GetValue<bool>("AutoPosition"))
                {
                    context.BuildsRefreshed += HandleBuildsRefreshedForAutoPosition;
                }
            };
        }


        private void HandleBuildsRefreshedForAutoPosition(object sender, Domain.Builds.BuildsRefreshedEventArgs e)
        {
            if (m_controller.enabled)
            {
                m_controller.ResetCamera();
            }

            Context.BuildsRefreshed -= HandleBuildsRefreshedForAutoPosition;
        }
    }            
}