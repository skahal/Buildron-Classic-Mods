using System;
using UnityEngine;
using Buildron.Domain.Mods;
using Buildron.Domain.CIServers;
using System.Collections;
using Skahal.Threading;
using Buildron.Domain.RemoteControls;
using System.Collections.Generic;
using Skahal.Common;

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
	public class CameraController : MonoBehaviour, IEventSubscriber
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
		private CameraState m_state = CameraState.ShowingBuilds;
		private Vector3 m_historyPosition;
		private bool m_autoPosition = true;
		private IModContext m_ctx;
		private Transform m_cameraTransform;
		#endregion

		#region Properties

		public Vector3 DistanceFromFocusBuild = new Vector3 (0, 0, -3);
		public Vector3 DistanceFromOriginalPosition = new Vector3 (0, 0, -1);
		public float AdjustPositionInterval = 0.02f;
		public float VelocityToShowTop = 0.1f;
		public float VelocityToShowSides = 0.1f;
		public float MinY = 0;

		#endregion

		#region Methods

		private void Awake ()
		{		
			m_ctx = Mod.Context;
			m_cameraTransform = m_ctx.Camera.MainCamera.transform;
			m_firstPosition = m_cameraTransform.position;
			m_historyPosition = m_originalPosition + new Vector3 (0, 30, 25);	

			m_originalPosition = m_loadedPosition = m_ctx.Data.GetValue<Vector3> ("CameraPosition");

			if (m_originalPosition == Vector3.zero) {
				m_originalPosition = new Vector3 (0f, 8f, -20f); 
			}

			m_targetPosition = m_originalPosition;

			m_ctx.Log.Warning ("Setting camera position to latest position: {0}", m_originalPosition);

			m_ctx.BuildFound += delegate {
				ChangeByBuilds ();
			};

			m_ctx.BuildRemoved += delegate {
				ChangeByBuilds ();
			};

			m_ctx.BuildsRefreshed += delegate {
				ChangeByBuilds ();
			};
	
			m_ctx.RemoteControlCommandReceived += (sender, e) => {
				var moveCmd = e.Command as MoveCameraRemoteControlCommand;

				if(moveCmd != null) {
					m_ctx.Log.Debug("Moving camera {0}...", moveCmd.Direction);
					Reposition(moveCmd.Direction);
					return;
				}

				var resetCmd = e.Command as ResetCameraRemoteControlCommand;

				if(resetCmd != null) {
					m_ctx.Log.Debug("Reseting camera...");
					ResetCamera();
				}
			};

			InitializePosition ();
			PrepareEffects ();
		}

		void OnEnable()
		{
			StartCoroutine (AdjustCameraPosition ());
		}

		void InitializePosition ()
		{
			m_state = CameraState.ShowingBuilds;

			if (m_originalPosition.z == 0) {
				m_cameraTransform.position = m_originalPosition;
				ResetCamera ();
			} else {
				m_targetPosition = m_originalPosition;
				m_autoPosition = m_ctx.Preferences.GetValue<bool>("AutoPosition");

				if (m_autoPosition) {
					ResetCamera ();
				}
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

				yield return new WaitForSeconds (AdjustPositionInterval);
			}
		}

		private void LateUpdate ()
		{
			m_cameraTransform.position = Vector3.Lerp (m_cameraTransform.position, m_targetPosition, Time.deltaTime);
		}

		private Vector3 CalculatePositionToShowAllBuilds ()
		{
			if (m_autoPosition) {
				var allBuilds = m_ctx.BuildGameObjects.GetAll ();
					
				var currentVisiblesCount = allBuilds.Length;
				var diff = m_lastVisiblesCount - currentVisiblesCount;
	
				if (diff > 0) {
					m_originalPosition = m_firstPosition;
				} else {
					var move = CalculateMoveBy(allBuilds);

					if (move == Vector3.zero) {
						var allUsers = m_ctx.UserGameObjects.GetAll ();
						move = CalculateMoveBy(allUsers);
					}

					m_originalPosition += move;
				}
							
				m_lastVisiblesCount = currentVisiblesCount;
			}
		
			return m_originalPosition;
		}

		private Vector3 CalculateMoveBy(IGameObjectController[] all)
		{
			var stopped = all.Stopped();
			var stoppedCount = stopped.Length;

			var y = 0f;
			float z = 0f;

			var fromTop = stopped.CountVisiblesFromTop();
			var fromRight = stopped.CountVisiblesFromRight();
			var fromBottom = stopped.CountVisiblesFromBottom();
			var fromLeft = stopped.CountVisiblesFromLeft();

			if (fromTop > fromBottom + 1) {
				y = -1;
			} else if (fromTop < fromBottom) {
				y = 1;
			} 

			if (fromLeft < stoppedCount || fromRight < stoppedCount) {
				z = -1;
			}

			return new Vector3 (
				0, 
				y * VelocityToShowTop,
				z * VelocityToShowSides);
		}
			

		private void ChangeByBuilds ()
		{
			var visibles = m_ctx.BuildGameObjects.GetAll ();

			if (visibles.Length == 1) {
				var visibleOne = visibles [0];
				m_target = visibleOne.gameObject.transform;
				m_state = CameraState.ShowingFocusBuild;
			} else {
				m_target = null;
				m_state = CameraState.ShowingBuilds;
			}
		}

		private void PrepareEffects ()
		{
			var cameraGO = m_ctx.Camera.MainCamera.gameObject;
			m_serverDownBlurEffect = m_ctx.GameObjects.AddComponent (cameraGO , "BlurEffect");	
			m_serverDownBlurEffect.enabled = true;

			m_serverDownToneMappingEffect = m_ctx.GameObjects.AddComponent (cameraGO, "Tonemapping");
			m_serverDownToneMappingEffect.enabled = true;

			m_ctx.CIServerStatusChanged += (e, args) => {
				var server = args.Server;

				if (server.Status == CIServerStatus.Down) {
					m_serverDownBlurEffect.enabled = true;
					m_serverDownToneMappingEffect.enabled = true;
				} else {
					m_serverDownBlurEffect.enabled = false;
					m_serverDownToneMappingEffect.enabled = false;
				}
			};
		}

		private void Reposition (Vector3 increment)
		{
			m_originalPosition += increment;        
			m_autoPosition = false;
			SaveCameraPosition ();
		}

		private void ResetCamera ()
		{
			m_originalPosition = Vector3.zero;
			m_autoPosition = true;
		}

		private void OnApplicationQuit ()
		{
			SaveCameraPosition ();
		}

		private void SaveCameraPosition ()
		{
			if (m_loadedPosition != m_originalPosition) {
				m_loadedPosition = m_originalPosition;
				m_ctx.Data.SetValue<Vector3> ("CameraPosition", m_originalPosition);
			}
		}

		#endregion
	}
}
