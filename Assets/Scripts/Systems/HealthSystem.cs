using System.Collections.Generic;
using Hud;
using Models;
using UnityEngine;
using Utilities.UI;

namespace Systems
{
    [DisallowMultipleComponent,RequireComponent(typeof(HealthStat))]
    public class HealthSystem : MonoBehaviour
    {
        [HideInInspector]
        public HealthStat healthStat;
        private readonly List<ActiveRateEffect> _expiredEffects = new List<ActiveRateEffect>();
        
        public static HealthSystem Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            healthStat = GetComponent<HealthStat>();
        }
        private void OnEnable()
        {
            if (healthStat != null)
                healthStat.OnValueChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            if (healthStat != null)
                healthStat.OnValueChanged -= OnHealthChanged;
        }

        private void Update()
        {
            float regenValue = healthStat.BaseDrainValue;
            _expiredEffects.Clear();

            foreach (var effect in healthStat.ActiveRateEffects)
            {
                regenValue += effect.RateAmount;
                effect.RemainingDuration -= Time.deltaTime;

                if (effect.RemainingDuration <= 0)
                    _expiredEffects.Add(effect);
            }

            healthStat.RequestChangeCurrentValue(regenValue * Time.deltaTime);

           
            foreach (var expired in _expiredEffects)
            {
                healthStat.RemoveEffect(expired);
            }
            
        }

        private void OnHealthChanged()
        {
            if (!CheckUpdateUI())return;
            
            CharacterStatsUI.Instance.OnHealthChanged?.Invoke(healthStat.CurrentValue,healthStat.MaxValue);
           // Debug.Log("Health changed");
        }
        private bool CheckUpdateUI()
        {
            var currentValue = healthStat.CurrentValue;
            var maxValue = healthStat.MaxValue;
            var currentBarValue = CharacterStatsUI.Instance.GetHealthBarValue();

            return StatUIUpdateChecker.ShouldUpdateUI(currentBarValue, currentValue, maxValue) ;
        }
    }
}
