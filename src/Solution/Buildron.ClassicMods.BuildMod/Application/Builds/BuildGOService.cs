using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buildron.Domain;
using UnityEngine;
using Zenject;
using Skahal.Logging;
using Buildron.Domain.Builds;
using Buildron.ClassicMods.BuildMod.Controllers;

namespace Buildron.ClassicMods.BuildMod.Application
{
    /// <summary>
    /// Application service to handle with build game objects.
    /// </summary>
    public class BuildGOService : GOServiceBase<IBuild, BuildController>
    {
        #region Methods
        public IList<GameObject> GetVisibles()
        {
            return GetVisiblesQuery().ToList();

        }

        public int CountVisibles()
        {
            return GetVisiblesQuery().Count();
        }

        public bool HasNotVisiblesFromTop()
        {
            var builds = GameObject.FindGameObjectsWithTag("Build").Select(b => b.GetComponent<BuildController>());

            return builds.Any(
                    b => b != null
                && b.HasReachGround
                && b.IsVisible
                && !b.IsHistoryBuild
                && Mathf.Abs(b.Rigidbody.velocity.y) <= b.VisibleMaxYVelocity
                && !b.TopCollider.IsVisibleFrom(Camera.main)
                && (b.LeftCollider.IsVisibleFrom(Camera.main)
                || b.RightCollider.IsVisibleFrom(Camera.main)
                || b.BottomCollider.IsVisibleFrom(Camera.main)));
        }

        public bool HasNotVisiblesFromSides()
        {
            var builds = GameObject.FindGameObjectsWithTag("Build").Select(b => b.GetComponent<BuildController>());

            return builds.Any(
                    b => b != null
                && b.HasReachGround
                && b.IsVisible
                && !b.IsHistoryBuild
				&& Mathf.Abs(b.Rigidbody.velocity.y) <= b.VisibleMaxYVelocity
                && b.TopCollider.IsVisibleFrom(Camera.main)
                && (!b.LeftCollider.IsVisibleFrom(Camera.main)
                || !b.RightCollider.IsVisibleFrom(Camera.main)
                || !b.BottomCollider.IsVisibleFrom(Camera.main)));
        }       

        public bool HasAllReachGround()
        {
            return GetVisibles().All(b => b.GetComponent<BuildController>().HasReachGround);
        }

		protected override string GetName (IBuild model)
		{
			return model.Id;
		}

		public override GameObject CreateGameObject (IBuild model)
		{
			var go = GetGameObject(model);

			if (go == null)
			{
                var prefab = Mod.Context.Assets.Load("BuildPrefab");
                go = Mod.Context.GameObjects.Create(prefab);
                Mod.DI.Container.InjectGameObject(go, true);
                var controller = go.GetComponent<BuildController>();
                controller.Model = model;
				go.name = GetName(model);
			}

			return go;
		}

		private IEnumerable<GameObject> GetVisiblesQuery()
		{
			var builds = GameObject.FindGameObjectsWithTag("Build").Select(b => b.GetComponent<BuildController>());

			return builds.Where(
				b => b != null
				&& b.Body != null
				&& b.IsVisible
				&& !b.IsHistoryBuild).Select(b => b.gameObject);
		}
		#endregion
    }
}
