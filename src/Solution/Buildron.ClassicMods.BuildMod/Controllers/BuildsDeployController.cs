using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Skahal.Logging;
using Zenject;
using Buildron.Domain.Builds;
using Buildron.Domain.CIServers;
using Buildron.ClassicMods.BuildMod.Application;

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
		private int m_deployedBuildsCount;
		private int m_currentTotemIndex;
		private Queue<GameObject> m_buildsToDeploy = new Queue<GameObject> ();
		public int m_totemsNumber = 2;
		#endregion

		#region Editor properties
		public Vector3 DeployCenterPosition = new Vector3 (-0.5f, 20, 1.5f);
		public float DeployInterval = 0.4f;
		public float TotemsDistance = 13;
		public Text BuildsCountLabel;
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
			Instance = this;
			m_container = Mod.Context.GameObjects.Create("Builds");
	
			Mod.Context.CIServerConnected += CIServerConnected;
		}

		private void CIServerConnected (object sender, CIServerConnectedEventArgs e)
		{
			Mod.Context.Log.Debug ("CIServerConnected.");
				
			// TODO: should come from some mod configuration screen.
			m_totemsNumber = Mod.Context.CIServer.BuildsTotemsNumber;

			m_initialDeployPosition = CalculateInitialPosition ();
			m_currentDeployPosition = m_initialDeployPosition;
		
			Mod.Context.BuildFound += (sender1, e1) => {		
				UpdateBuild (e1.Build);
			};

			Mod.Context.BuildRemoved += (object sender2, BuildRemovedEventArgs e2) => {		
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
				m_deployedBuildsCount--;            
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
					m_deployedBuildsCount++;

					// TODO: how does it will be defined by mods?
					//BuildsCountLabel.text = string.Format ("Builds\n{0}", m_deployedBuildsCount);
				}
		
				yield return new WaitForSeconds (DeployInterval);	
			}
		}
	}
}