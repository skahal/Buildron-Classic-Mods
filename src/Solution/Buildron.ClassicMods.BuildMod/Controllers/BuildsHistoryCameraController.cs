using UnityEngine;
using Skahal.Threading;
using Skahal.Common;

namespace Buildron.ClassicMods.BuildMod.Controllers
{
    public class BuildsHistoryCameraController : MonoBehaviour, IEventSubscriber
    {
		private bool m_showingHistory;
		private Vector3 m_originalPosition;
		private Vector3 m_targetPosition;
        private Vector3 m_historyPosition;
		private Transform m_cameraTransform;

		public Vector3 DistanceFromHistory = new Vector3 (0, 0, -11);

		void Awake()
		{
			m_cameraTransform = Mod.Context.Camera.MainCamera.transform;
		}

		public void ShowHistory ()
		{
			var histories = BuildsHistoryController.GetAll ();
			
			if (histories.Length > 0) {
				
				Mod.Context.UI.SetStatusText ("Today's builds history");
				m_showingHistory = true;
				m_originalPosition = transform.position;
                m_historyPosition = m_originalPosition + new Vector3(0, 30, 25);

                SHCoroutine.Start (2f, () => {
					SHCoroutine.Loop (1.5f, 0, histories.Length, (t) => {
						if (m_showingHistory) {
							m_targetPosition = histories [histories.Length - 1 - Mathf.FloorToInt (t)].transform.position + DistanceFromHistory; 
							return true;
						}
						
						return false;
					});
				});
			}
		}

	    public void ShowBuilds()
		{
			Mod.Context.UI.SetStatusText(string.Empty);
			m_showingHistory = false;
			m_targetPosition = m_originalPosition;

			SHCoroutine.Start (3f, () => {
                Mod.Context.Camera.UnregisterController<BuildsHistoryCameraController>();
            });
		}

		private void LateUpdate ()
		{
			m_cameraTransform.position = Vector3.Lerp (m_cameraTransform.position, m_targetPosition, Time.deltaTime);
		}
	}
}
