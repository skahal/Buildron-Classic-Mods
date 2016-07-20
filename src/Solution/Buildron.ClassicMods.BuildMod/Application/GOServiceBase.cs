using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buildron.Domain;
using UnityEngine;
using Zenject;
using Skahal;

namespace Buildron.ClassicMods.BuildMod.Application
{
    /// <summary>
    /// Base class to Game Object application services.
    /// </summary>
    public abstract class GOServiceBase<TModel, TController>  
		where TController : class, ISHController<TModel>
    {
        #region Methods
        protected abstract string GetName(TModel model);

        public virtual GameObject GetGameObject(TModel model)
        {
            return GameObject.Find(GetName(model));
        }

        public virtual bool ExistsGameObject(TModel model)
        {
            return GetGameObject(model) != null;
        }

        public abstract GameObject CreateGameObject(TModel model);     
        #endregion
    }
}
