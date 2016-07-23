using System;
using UnityEngine;
using Buildron.Domain.Mods;
using Buildron.Domain.CIServers;
using System.Collections;
using Skahal.Threading;

namespace Buildron.ClassicMods.CameraMod
{
	#region Enums
	/// <summary>
	/// Camera state.
	/// </summary>
	public enum CameraState
	{
		ShowingBuilds,
		ShowingFocusBuild,
		GoingToHistory,
		GoingToBuilds,
		ShowingHistory,
		ShowingConfigPanel
	}
	#endregion

	/// <summary>
	/// Camera controller.
	/// </summary>
	//[RequireComponent(typeof(BlurEffect))]
	public class CameraController : MonoBehaviour//, IInitializable
	{
		#region Fields
		private Vector3 m_firstPosition;
		private int m_lastVisiblesCount;
		private Vector3 m_loadedPosition;
		private Vector3 m_originalPosition;
		private Vector3 m_targetPosition;
		private Transform m_target;
		private MonoBehaviour m_serverDownBlurEffect;
		private MonoBehaviour m_serverDownToneMappingEffect;
		private CameraState m_state = CameraState.ShowingConfigPanel;
		private Vector3 m_historyPosition;
		private bool m_autoPosition = true;
		private IModContext m_ctx;
		#endregion

		#region Properties
		public int MaxBuildsBeforeMove = 16;
		public Vector3 DistanceFromFocusBuild = new Vector3(0, 0, -3);
		public Vector3 DistanceFromOriginalPosition = new Vector3(0, 0, -1);
		public Vector3 DistanceFromHistory = new Vector3 (0, 0, -11);
		public float BuildHeight = 0.5f;
		public float BuildWidth = 2.5f;
		public float DiffYMoveEachVisibleBuild = 0.3f;
		public float DiffZMoveEachVisibleBuild = 0.6f;
		public float AdjustPositionInterval = 1f;
		public float VelocityToShowTop = 0.1f;
		public float VelocityToShowSides = 0.1f;
		public float MinY = 0;
		#endregion

		#region Methods
		private void Awake ()
		{		
			m_ctx = Mod.Context;
			m_firstPosition = m_originalPosition;
			m_targetPosition = transform.position;
			m_historyPosition = m_originalPosition + new Vector3 (0, 30, 25);	

			m_originalPosition = m_loadedPosition = m_ctx.Data.GetValue<Vector3> ("CameraPosition");

			if (m_originalPosition == Vector3.zero) {
				m_originalPosition = new Vector3 (2.22f, 6f, -13f); 
			}


			m_ctx.Log.Warning ("Setting camera position to latest position: {0}", m_originalPosition);

			Messenger.Register (gameObject, 
				"OnBuildReachGround",
				"OnBuildHidden",
				"OnBuildVisible", 
				"OnShowHistoryRequested",
				"OnShowBuildsRequested",
				"OnServerDown",
				"OnZoomIn",
				"OnZoomOut",
				"OnGoLeft",
				"OnGoRight",
				"OnGoUp",
				"OnGoDown",
				"OnResetCamera",
				"OnBuildFilterUpdated");
	
			m_ctx.CIServerConnected += HandleCIServerConnected;
			PrepareEffects ();
			StartCoroutine (AdjustCameraPosition ());
		}

		void HandleCIServerConnected (object sender, CIServerConnectedEventArgs e)
		{
			m_state = CameraState.ShowingBuilds;

			if (m_originalPosition.z == 0)
			{
				transform.position = m_originalPosition;
				OnResetCamera ();
			}
			else
			{
				m_targetPosition = m_originalPosition;
				m_autoPosition = false;
			}
		}

		private IEnumerator AdjustCameraPosition ()
		{
			while (true) {
				switch (m_state) {
				case CameraState.ShowingBuilds:
					m_targetPosition = CalculatePositionToShowAllBuilds ();
					break;

				case CameraState.ShowingFocusBuild:
					m_targetPosition = m_target.position + DistanceFromFocusBuild;
					break;

				case CameraState.GoingToHistory:
					m_targetPosition = m_historyPosition;
					break;

				case CameraState.GoingToBuilds:
					m_targetPosition = m_originalPosition;
					break;
				}	

				if (m_targetPosition.y < MinY) {
					m_targetPosition.y = MinY;	
				}

				yield return new WaitForSeconds(AdjustPositionInterval);
			}
		}

		private void LateUpdate ()
		{
			transform.position = Vector3.Lerp (transform.position, m_targetPosition, Time.deltaTime);
		}

		private Vector3 CalculatePositionToShowAllBuilds ()
		{
			// TODO: see this after move builds to BuildMod
			//		if (m_autoPosition) {
			//			var currentVisiblesCount = BuildGOService.CountVisibles();
			//			var diff = m_lastVisiblesCount - currentVisiblesCount;
			//		
			//			if (diff > 0) {
			//				m_originalPosition = m_firstPosition;
			//			} else {
			//				var hasNotVisiblesFromTop = false;
			//				var hasNotVisiblesFromSides = BuildGOService.HasNotVisiblesFromSides ();
			//
			//				if (!hasNotVisiblesFromSides) {
			//					hasNotVisiblesFromTop = BuildGOService.HasNotVisiblesFromTop ();
			//				}
			//			
			//				m_originalPosition += new Vector3 (
			//													0, 
			//													hasNotVisiblesFromTop ? VelocityToShowTop : 0, 
			//													hasNotVisiblesFromSides ? -VelocityToShowSides : 0);
			//			}
			//		
			//			m_lastVisiblesCount = currentVisiblesCount;
			//        }

			return m_originalPosition;
		}

		private void OnBuildHidden ()
		{
			ChangeByBuilds ();		
		}

		private void OnBuildVisible ()
		{
			ChangeByBuilds ();		
		}

		private void ChangeByBuilds ()
		{
			// TODO: see this after move builds to BuildMod
			//		if (BuildGOService.CountVisibles() == 1) {
			//			var visibleOne = BuildGOService.GetVisibles () [0];
			//			m_target = visibleOne.transform;
			//			m_state = CameraState.ShowingFocusBuild;
			//			Messenger.Send ("OnCameraZoomIn");
			//		} else {
			//			m_target = null;
			//			m_state = CameraState.ShowingBuilds;
			//			Messenger.Send ("OnCameraZoomOut");
			//		}
		}

		private void PrepareEffects ()
		{
			m_serverDownBlurEffect = m_ctx.GameObjects.AddComponent (gameObject, "BlurEffect");	
			m_serverDownBlurEffect.enabled = true;

			m_serverDownToneMappingEffect = m_ctx.GameObjects.AddComponent (gameObject, "Tonemapping");
			m_serverDownToneMappingEffect.enabled = true;

			m_ctx.CIServerStatusChanged += (e, args) => {
				var server = args.Server;

				if (server.Status == CIServerStatus.Down)
				{
					m_serverDownBlurEffect.enabled = true;
					m_serverDownToneMappingEffect.enabled = true;
				}
				else
				{
					m_serverDownBlurEffect.enabled = false;
					m_serverDownToneMappingEffect.enabled = false;
				}
			};
		}

		private void OnShowHistoryRequested ()
		{
			// TODO: see this after move builds to BuildMod
			//		var histories = BuildsHistoryController.GetAll ();
			//		
			//		if (histories.Length > 0) {
			//			m_state = CameraState.GoingToHistory;
			//			
			//			StatusBarController.SetStatusText ("Today's builds history");
			//			
			//			SHCoroutine.Start (2f, () => 
			//			{
			//				SHCoroutine.Loop (1.5f, 0, histories.Length, (t) => 
			//				{
			//					if (m_serverService.GetState().IsShowingHistory) {
			//						m_state = CameraState.ShowingHistory;
			//						m_targetPosition = histories [histories.Length - 1 - Mathf.FloorToInt (t)].transform.position + DistanceFromHistory; 
			//						return true;
			//					}
			//					
			//					return false;
			//				});
			//			});
			//		}
		}

		private void OnShowBuildsRequested ()
		{
			m_state = CameraState.GoingToBuilds;

			// TODO: maybe should go to IUIProxy.
			//StatusBarController.ClearStatusText ();

			SHCoroutine.Start (3f, () => 
			{
				ChangeByBuilds ();
			});
		}

		private void Reposition (Vector3 increment)
		{
			m_originalPosition += increment;        
			m_autoPosition = false;
			SaveCameraPosition();
		}

		private void OnZoomIn ()
		{
			Reposition (Vector3.forward);
		}

		private void OnZoomOut ()
		{
			Reposition (Vector3.back);
		}

		private void OnGoLeft ()
		{
			Reposition (Vector3.left);
		}

		private void OnGoRight ()
		{
			Reposition (Vector3.right);
		}

		private void OnGoUp ()
		{
			Reposition (Vector3.up);
		}

		private void OnGoDown ()
		{
			Reposition (Vector3.down);
		}

		private void OnResetCamera ()
		{
			m_originalPosition = m_firstPosition;
			m_autoPosition = true;
		}

		private void OnApplicationQuit()
		{
			SaveCameraPosition();
		}

		private void SaveCameraPosition()
		{
			if (m_loadedPosition != m_originalPosition)
			{
				m_loadedPosition = m_originalPosition;
				m_ctx.Data.SaveValue<Vector3> ("CameraPosition", m_originalPosition);
			}
		}
		#endregion
	}
}
