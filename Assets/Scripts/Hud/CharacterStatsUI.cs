using System;
using DG.Tweening;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Hud
{
    public class CharacterStatsUI : MonoBehaviour
    {
        [SerializeField]private Color fullHealthColor;
        [SerializeField]private Color fullHungerColor;
        [SerializeField]private Color fullEnergyColor;
        
        [SerializeField]private Color lowHealthColor;
        [SerializeField]private Color lowHungerColor;
        [SerializeField]private Color lowEnergyColor;
        
        [SerializeField]private Slider healthBar;
        [SerializeField]private Slider hungerBar;
        [SerializeField]private Slider energyBar;
        
        [SerializeField]private Image healthImage;
        [SerializeField]private Image hungerImage;
        [SerializeField]private Image energyImage;
        
        private StatBarData _healthBarData;
        private StatBarData _hungerBarData;
        private StatBarData _energyBarData;
        
        [SerializeField] private float animationDuration = 0.5f;
    
        public Action<float,float> OnHealthChanged;
        public Action<float,float> OnHungerChanged;
        public Action<float,float> OnEnergyChanged;

        public static CharacterStatsUI Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;

            _healthBarData = new StatBarData(healthBar, healthImage, lowHealthColor, fullHealthColor);
            _hungerBarData = new StatBarData(hungerBar, hungerImage, lowHungerColor, fullHungerColor);
            _energyBarData = new StatBarData(energyBar, energyImage, lowEnergyColor, fullEnergyColor);

            AnimateStatBar(_healthBarData, HealthSystem.Instance.healthStat.CurrentValue, HealthSystem.Instance.healthStat.MaxValue);
            AnimateStatBar(_hungerBarData, HungerSystem.Instance.hungerStat.CurrentValue, HungerSystem.Instance.hungerStat.MaxValue);
            AnimateStatBar(_energyBarData, EnergySystem.Instance.energyStat.CurrentValue, EnergySystem.Instance.energyStat.MaxValue);
        }

        private void OnEnable()
        {
            OnHealthChanged += ChangeHealthStatus;
            OnHungerChanged += ChangeHungerStatus;
            OnEnergyChanged += ChangeEnergyStatus;
        }
        private void OnDisable()
        {
            OnHealthChanged -= ChangeHealthStatus;
            OnHungerChanged -= ChangeHungerStatus;
            OnEnergyChanged -= ChangeEnergyStatus;
        }

        private void ChangeHealthStatus(float health, float maxHealth)
        {
            AnimateStatBar(_healthBarData, health, maxHealth);
        }
        private void ChangeHungerStatus(float hunger, float maxHunger)
        {
            AnimateStatBar(_hungerBarData, hunger, maxHunger);
        }
        private void ChangeEnergyStatus(float energy, float maxEnergy)
        {
            AnimateStatBar(_energyBarData, energy, maxEnergy);
        }

        private void AnimateStatBar(StatBarData data, float value, float maxValue)
        {
            float targetNormalized = Mathf.Clamp01(value / maxValue);

            DOTween.To(() => data.bar.maxValue, x => data.bar.maxValue = x, maxValue, animationDuration);
            DOTween.To(() => data.bar.value, x => data.bar.value = x, value, animationDuration);
            DOTween.To(() => data.fillImage.color,
                x => data.fillImage.color = x,
                Color.Lerp(data.lowColor, data.fullColor, targetNormalized),
                animationDuration);
        }
        public float GetHungerBarValue()
        {
            return _hungerBarData.bar.value;
        } 
        public float GetHealthBarValue()
        {
            return _healthBarData.bar.value;
        }
        public float GetEnergyBarValue()
        {
            return _energyBarData.bar.value;
        }
    
    }
}
