using System;
using Buildron.Domain.Mods;
using UnityEngine;

namespace Buildron.ClassicMods.EnvironmentMod
{
	public class Mod : IMod
	{
		public void Initialize (IModContext context)
		{
			context.CreateGameObjectFromPrefab ("EnvironmentPrefab");

			var cam = context.Camera.MainCamera;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = Color.black;
		}
	}
}

