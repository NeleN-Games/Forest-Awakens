using System;
using Databases;
using Interfaces;
using Models;
using Models.Data;
using Services;
using UnityEngine;

namespace Managers
{
    public abstract class Crafter<TEnum, TData, TDatabase> : MonoBehaviour,IInitializable
        where TEnum : Enum
        where TData : CraftableAssetData<TEnum>
        where TDatabase : GenericDatabase<TEnum, TData>
    {
        public Action<CraftCommand<TEnum>> OnCraft;

        protected TDatabase Database;
        
        public abstract void Initialize();

        public abstract void OnDestroy();
       

        protected void Craft(CraftCommand<TEnum> command)
        {
            var data = Database.Get(command.ID);
            if (data == null)
            {
                OnCraftFailure(null);
                return;
            }

            if (ServiceLocator.Get<PlayerInventory>().HasEnoughSources(data.GetRequirements()))
            {
                HandleCraftSuccess(data);
                OnCraftSuccess(data);
            }
            else
            {
                OnCraftFailure(data);
            }
        }

        protected abstract void HandleCraftSuccess(TData data);
        protected abstract void OnCraftSuccess(TData data);
        protected abstract void OnCraftFailure(TData data);
      
    }
}