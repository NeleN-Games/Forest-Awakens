using System;
using Databases;
using Interfaces;
using Models;
using Models.Data;
using Services;
using UnityEngine;

namespace Managers
{
    public abstract class Crafter<TEnum, TData, TDatabase> : MonoBehaviour
        where TEnum : Enum
        where TData : CraftableAssetData<TEnum>, ICraftable<TEnum>, IIdentifiable<TEnum>
        where TDatabase : GenericDatabase<TEnum, TData>
    {
        public Action<CraftCommand<TEnum>, TDatabase> OnCraft;

        public TDatabase database;

        private void Awake()
        {
            database = ServiceLocator.Get<TDatabase>();
            OnCraft += Craft;
        }

        private void OnDestroy()
        {
            OnCraft -= Craft;
        }

        private void Craft(CraftCommand<TEnum> command, TDatabase db)
        {
            var data = database.Get(command.ID);
            if (data == null)
            {
                OnCraftFailure(null);
                return;
            }

            if (PlayerInventory.Instance.HasEnoughSources(data.resourceRequirements))
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