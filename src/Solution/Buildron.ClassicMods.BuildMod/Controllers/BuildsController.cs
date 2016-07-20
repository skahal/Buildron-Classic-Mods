using UnityEngine;
using Zenject;
using Buildron.ClassicMods.BuildMod.Application;
using Skahal.Threading;

namespace Buildron.ClassicMods.BuildMod.Controllers
{
	/// <summary>
	/// Builds controller.
	/// </summary>
	public class BuildsController : MonoBehaviour
	{
		#region Fields
	    [Inject]
		private BuildGOService m_buildGOService;
		#endregion

		#region Methods
	    void Start()
	    {
			Mod.Context.BuildFound += delegate { HandleVisibleBuilds(); };

			Mod.Context.BuildRemoved += delegate {
				m_buildGOService.WakeUpSleepingBuilds();
				HandleVisibleBuilds();
			};
	    }

		private void HandleVisibleBuilds ()
		{
			SHCoroutine.Start(1f, () => {
				var builds = m_buildGOService.GetVisibles ();

				if (builds.Count == 1) {
					builds [0].GetComponentInChildren<BuildFocusedPanelController> ().Show ();
				} else {
					foreach (var b in builds) {
						b.GetComponentInChildren<BuildFocusedPanelController> ().Hide ();
					}
				}
			});
		}
		#endregion
	}
}