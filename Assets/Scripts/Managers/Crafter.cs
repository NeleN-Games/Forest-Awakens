using System;
using Databases;
using Interfaces;
using Models;
using Models.Data;
using UnityEngine;

namespace Managers
{
    public abstract class Crafter<TID, TData, TDatabase> : MonoBehaviour
        where TID : Enum
        where TData : CraftableData, ICraftable<TID>, IIdentifiable<TID>
        where TDatabase : GenericDatabase<TID, TData>
    {
        public Action<CraftCommand<TID>, TDatabase> OnCraft;

        [SerializeField] private TDatabase database;

        private void Awake()
        {
            OnCraft += Craft;
        }

        private void OnDestroy()
        {
            OnCraft -= Craft;
        }

        private void Craft(CraftCommand<TID> command, TDatabase db)
        {
            var data = GenericDatabase<TID, TData>.Get(command.ID);
            if (data == null)
            {
                OnCraftFailure(null);
                return;
            }

            if (PlayerInventory.Instance.HasEnoughSources(data.requirements))
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