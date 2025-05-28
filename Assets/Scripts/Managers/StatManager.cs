using System;
using System.Collections.Generic;
using Base_Classes;
using Enums;
using Models.Scriptable_Objects;
using Systems;
using UnityEngine;

namespace Managers
{
    public class StatManager : MonoBehaviour
    {
        public static StatManager Instance;

        public HungerSystem hungerSystem;
        public EnergySystem energySystem;
        public HealthSystem healthSystem;

        private Dictionary<StatType, StatBase> _statMap;

        private void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                }
            
                _statMap = new Dictionary<StatType, StatBase>()
                {
                    { StatType.Health, healthSystem.healthStat  },
                    { StatType.Energy, energySystem.energyStat },
                    { StatType.Hunger, hungerSystem.hungerStat}
                };
            }
            catch (Exception e)
            {
               Debug.LogError("Can not Set Stat Manager");
            }
          
        }

        public void ApplyModifier(StatModifierSo modifier)
        {
            foreach (var effect in modifier.effects)
            {
                if (!_statMap.TryGetValue(effect.statType, out StatBase stat)) continue;
                
                if (effect.hasEffectRate)
                {
                    stat.RequestApplyEffect(effect);
                }
                Debug.Log($"Requesting to change Current Value to: {effect.amount}");
                stat.RequestChangeCurrentValue(effect.amount);
            }
        }
    }
}
