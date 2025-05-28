using Base_Classes;
using Models.Scriptable_Objects;
using Systems;
using UnityEngine;

namespace Models
{
    [RequireComponent(typeof(EnergySystem))]
    public class EnergyStat : StatBase
    {
        protected override void ModifyStat(StatEffect effect)
        {
            base.ModifyStat(effect);
        }
    }
}
