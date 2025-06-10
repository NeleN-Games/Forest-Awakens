using System.Collections.Generic;
using Hud;
using Models;
using UnityEngine;
using Utilities.UI;

namespace Systems
{
    [DisallowMultipleComponent,RequireComponent(typeof(HungerStat))]
    public class HungerSystem : MonoBehaviour
    {
        [HideInInspector]
        public HungerStat hungerStat;
        private readonly List<ActiveRateEffect> _expiredEffects = new List<ActiveRateEffect>();

        public static HungerSystem Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            hungerStat = GetComponent<HungerStat>();
        }

        private void OnEnable()
        {
            if (hungerStat != null)
                hungerStat.OnValueChanged += OnHungerChanged;
        }

        private void OnDisable()
        {
            if (hungerStat != null)
                hungerStat.OnValueChanged -= OnHungerChanged;
        }

        private void Update()
        {
            float regenValue = hungerStat.BaseDrainValue;
            _expiredEffects.Clear();

            foreach (var effect in hungerStat.ActiveRateEffects)
            {
                regenValue += effect.RateAmount;
                effect.RemainingDuration -= Time.deltaTime;

                if (effect.RemainingDuration <= 0)
                    _expiredEffects.Add(effect);
            }

            hungerStat.RequestChangeCurrentValue(regenValue * Time.deltaTime);

            foreach (var expired in _expiredEffects)
            {
                hungerStat.RemoveEffect(expired);
            }
            
        }

        private void OnHungerChanged()
        {
            if (!CheckUpdateUI()) return;
            
            CharacterStatsUI.Instance.OnHungerChanged?.Invoke(hungerStat.CurrentValue,hungerStat.MaxValue);
          //  Debug.Log("Hunger changed");
        }

        private bool CheckUpdateUI()
        {
            var currentValue = hungerStat.CurrentValue;
            var maxValue = hungerStat.MaxValue;
            var currentBarValue = CharacterStatsUI.Instance.GetHungerBarValue();

            return StatUIUpdateChecker.ShouldUpdateUI(currentBarValue, currentValue, maxValue) ;
        }
    }
}
