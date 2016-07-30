using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Skahal.Logging;
using Zenject;
using Buildron.Domain.Builds;
using Buildron.Domain.CIServers;
using Buildron.ClassicMods.BuildMod.Application;
using Buildron.Domain.Mods;

namespace Buildron.ClassicMods.BuildMod.Controllers
{
	/// <summary>
	/// Builds deploy controller.
	/// </summary>
	public class BuildsDeployController : MonoBehaviour
	{
		#region Fields
		private GameObject m_container;
		private Vector3 m_initialDeployPosition;
		private Vector3 m_currentDeployPosition;
		private int m_currentTotemIndex;
		private Queue<GameObject> m_buildsToDeploy = new Queue<GameObject> ();
		public int m_totemsNumber = 2;
        private IModContext m_ctx;
		#endregion

		#region Editor properties
		public Vector3 DeployCenterPosition = new Vector3 (0f, 20, 1.5f);
		public float DeployInterval = 0.4f;
		public float TotemsDistance = 13;		
		#endregion

		#region Properties

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static BuildsDeployController Instance { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has builds to deploy.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has builds to deploy; otherwise, <c>false</c>.
		/// </value>
		public bool HasBuildsToDeploy {
			get {
				return m_buildsToDeploy.Count > 0;
			}
		}

		[Inject]
		public BuildGOService Service { get; set; }

		#endregion

		private void Awake ()
		{
            m_ctx = Mod.Context;
            Instance = this;
			m_container = m_ctx.GameObjects.Create("Builds");

            m_ctx.CIServerConnected += CIServerConnected;
		}

		private void CIServerConnected (object sender, CIServerConnectedEventArgs e)
		{
            m_totemsNumber = m_ctx.Preferences.GetValue<int>("BuildsTotemsNumber");
			m_initialDeployPosition = CalculateInitialPosition ();
			m_currentDeployPosition = m_initialDeployPosition;

            m_ctx.BuildFound += (sender1, e1) => {		
				UpdateBuild (e1.Build);
			};

            m_ctx.BuildRemoved += (object sender2, BuildRemovedEventArgs e2) => {		
				RemoveBuild (e2.Build);
			};
			
			StartCoroutine (DeployBuilds ());
		}

		private void UpdateBuild (IBuild b)
		{
			GameObject go;

			if (Service.ExistsGameObject (b)) {
				SHLog.Debug ("BuildsDeploy: existing build updated {0}", b.Id);

				go = Service.GetGameObject (b);
				go.SendMessage ("Show");
			} else {
				SHLog.Debug ("BuildsDeploy: new build updated {0}", b.Id);

				go = Service.CreateGameObject (b);
				go.transform.parent = m_container.transform;						
			}

            if (float.IsNaN(m_initialDeployPosition.x))
            {
                m_initialDeployPosition.x = 0;
            }

            m_currentDeployPosition.x = m_initialDeployPosition.x + (m_currentTotemIndex * TotemsDistance);
            go.transform.position = m_currentDeployPosition;
			go.SetActive (false);
			m_buildsToDeploy.Enqueue (go);

			m_currentTotemIndex++;
		
			if (m_currentTotemIndex >= m_totemsNumber) {
				m_currentTotemIndex = 0;
				m_initialDeployPosition = CalculateInitialPosition ();
			}
		
			m_currentDeployPosition += Vector3.up;
		}

		private void RemoveBuild (IBuild b)
		{
			if (Service.ExistsGameObject (b)) {
				var go = Service.GetGameObject (b);
				go.SendMessage ("Hide");
			}
		}

		private Vector3 CalculateInitialPosition ()
		{
			var intialPosition = DeployCenterPosition;
			var totemsNumberModifier = m_totemsNumber / 2;
			var totemsDistanceModifier = TotemsDistance;
		
			if (m_totemsNumber % 2 == 0) {
				totemsDistanceModifier -= TotemsDistance / m_totemsNumber;
			}	
		
			intialPosition.x = intialPosition.x - totemsNumberModifier * totemsDistanceModifier;
	
			return intialPosition;
		}

		private IEnumerator DeployBuilds ()
		{
			while (true) {
				if (m_buildsToDeploy.Count > 0) {
					m_buildsToDeploy.Dequeue ().SetActive (true);
				}
		
				yield return new WaitForSeconds (DeployInterval);	
			}
		}
	}
}