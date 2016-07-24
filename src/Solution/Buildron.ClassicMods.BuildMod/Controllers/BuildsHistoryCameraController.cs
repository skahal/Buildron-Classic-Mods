using System;
using UnityEngine;
using Buildron.Domain.RemoteControls;
using Skahal.Threading;

namespace Buildron.ClassicMods.BuildMod.Controllers
{
	public class BuildsHistoryCameraController : MonoBehaviour
	{
		private bool m_showingHistory;
		private Vector3 m_originalPosition;
		private Vector3 m_targetPosition;

		public Vector3 DistanceFromHistory = new Vector3 (0, 0, -11);

		void Start ()
		{	
			Mod.Context.RemoteControlCommandReceived += (sender, e) => {
				var cmd = e.Command as CustomRemoteControlCommand;

				if (cmd != null) {
					switch (cmd.Name) {
					case "ShowHistory":
						ShowHistory ();
						break;

					case "ShowBuilds":
						ShowBuilds ();
						break;
					}
				}
			};
		}

		private void ShowHistory ()
		{
			// TODO: see this after move builds to BuildMod
			var histories = BuildsHistoryController.GetAll ();
			
			if (histories.Length > 0) {

				Mod.Context.UI.SetStatusText ("Today's builds history");
				m_showingHistory = true;
				m_originalPosition = transform.position;

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

		private void ShowBuilds ()
		{
			Mod.Context.UI.SetStatusText(string.Empty);

			m_targetPosition = m_originalPosition;

			SHCoroutine.Start (3f, () => {
				m_showingHistory = false;
			});
		}

		private void LateUpdate ()
		{
			// TODO: see how not conflitc with others mods trying to change Camera. Maybe a CameraProxy?
			if (m_showingHistory) {
				transform.position = Vector3.Lerp (transform.position, m_targetPosition, Time.deltaTime);
			}
		}
	}
}
