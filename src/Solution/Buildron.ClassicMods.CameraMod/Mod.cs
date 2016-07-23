using System;
using Buildron.Domain.Mods;
using UnityEngine;

namespace Buildron.ClassicMods.CameraMod
{
	public class Mod : IMod
	{
		public static IModContext Context { get; private set; }

		public void Initialize (IModContext context)
		{
			Context = context;
			context.GameObjects.AddComponent<CameraController> (Camera.main.gameObject);
		}
	}
}
