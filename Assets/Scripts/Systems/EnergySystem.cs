using System;
using System.Collections.Generic;
using Hud;
using Models;
using UnityEngine;
using Utilities.UI;

namespace Systems
{
    [DisallowMultipleComponent,RequireComponent(typeof(EnergyStat))]
    public class EnergySystem : MonoBehaviour
    {
        [HideInInspector]
        public EnergyStat energyStat;
        private readonly List<ActiveRateEffect> _expiredEffects = new List<ActiveRateEffect>();

        public static EnergySystem Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            energyStat = GetComponent<EnergyStat>();
        }

        private void OnEnable()
        {
            if (energyStat != null)
                energyStat.OnValueChanged += OnEnergyChanged;
        }

        private void OnDisable()
        {
            if (energyStat != null)
                energyStat.OnValueChanged -= OnEnergyChanged;
        }

        private void Update()
        {
            float regenValue = energyStat.BaseDrainValue;
            _expiredEffects.Clear();

            foreach (var effect in energyStat.ActiveRateEffects)
            {
                regenValue += effect.RateAmount;
                effect.RemainingDuration -= Time.deltaTime;

                if (effect.RemainingDuration <= 0)
                    _expiredEffects.Add(effect);
            }

            energyStat.RequestChangeCurrentValue(regenValue * Time.deltaTime);

            foreach (var expired in _expiredEffects)
            {
                energyStat.RemoveEffect(expired);
            }
            
        }

        private void OnEnergyChanged()
        {
            if (!CheckUpdateUI()) return;
            
            CharacterStatsUI.Instance.OnEnergyChanged?.Invoke(energyStat.CurrentValue,energyStat.MaxValue);
           // Debug.Log("Energy changed");
        }
        
        private bool CheckUpdateUI()
        {
            var currentValue = energyStat.CurrentValue;
            var maxValue = energyStat.MaxValue;
            var currentBarValue = CharacterStatsUI.Instance.GetEnergyBarValue();

            return StatUIUpdateChecker.ShouldUpdateUI(currentBarValue, currentValue, maxValue) ;
        }
    }
}
