using System;
using Buildron.Domain.Mods;
using UnityEngine;
using Skahal.Common;

namespace Buildron.Infrastructure.GameObjectsProxies
{
	/// <summary>
	/// Mod game objects proxy.
	/// </summary>
	public class ModGameObjectsProxy : IGameObjectsProxy
	{
		#region Fields
		private GameObject m_modRoot;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Buildron.Infrastructure.GameObjectsProxies.ModGameObjectsProxy"/> class.
		/// </summary>
		public ModGameObjectsProxy ()
		{
			m_modRoot = new GameObject ("Mod");
		}        
        #endregion

        #region Methods
        public TComponent Create<TComponent> (string name = null, Transform parent = null) where TComponent : Component
		{
			var go = new GameObject (name ?? typeof(TComponent).Name);
            SetParent(go, parent);

			return go.AddComponent<TComponent> ();
		}

        public GameObject Create(UnityEngine.Object prefab, Transform parent = null)
        {
            var go = GameObject.Instantiate(prefab) as GameObject;
            SetParent(go, parent);

            return go;
        }

        private void SetParent(GameObject go, Transform parent)
        {
            if (parent == null)
            {
                go.transform.parent = m_modRoot.transform;
            }
            else
            {
                go.transform.parent = parent;
            }
        }
        #endregion
    }
}
