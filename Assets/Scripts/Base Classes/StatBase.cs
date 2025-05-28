using System;
using System.Collections.Generic;
using DG.Tweening;
using Models;
using Models.Scriptable_Objects;
using UnityEngine;

namespace Base_Classes
{
    public abstract class StatBase : MonoBehaviour
    {
        [SerializeField]
        private float baseDrainValue=1;

        public float BaseDrainValue=> baseDrainValue;
        private float _maxValue;
        public float MaxValue => _maxValue;
        
        private float _currentValue;
        public float CurrentValue => _currentValue;
        public event Action OnValueChanged;

        private List<ActiveRateEffect> _activeRateEffects = new List<ActiveRateEffect>();
        public IReadOnlyList<ActiveRateEffect> ActiveRateEffects => _activeRateEffects; 
        private void Awake()
        {
            _currentValue = 50;
            _maxValue = 150;
        }

        public void RequestChangeCurrentValue(float value)
        {            
            ChangeCurrentValue(value);
        }

        protected virtual void ChangeCurrentValue(float value)
        {
            _currentValue = Mathf.Clamp(_currentValue + value, 0,MaxValue);
            OnValueChanged?.Invoke();
        }

        public void RequestChangeMaxValue(float value)
        {
            ChangeMaxValue(value);
        }

        protected virtual void ChangeMaxValue(float value)
        {
            _maxValue = Mathf.Clamp(_maxValue + value, 0,float.MaxValue);
            OnValueChanged?.Invoke();
        }
        public void RequestApplyEffect(StatEffect effect)
        {
            if (effect.hasEffectRate)
            {
                AddEffect(effect);
            }
        }

        protected virtual void AddEffect(StatEffect effect)
        {
            var newEffect = new ActiveRateEffect( effect.rateAmount,effect.duration);
            _activeRateEffects.Add(newEffect);
        }
        public void RemoveEffect(ActiveRateEffect effect)
        {
            _activeRateEffects.Remove(effect);
        }
        protected virtual void ModifyStat(StatEffect effect)
        {
            ChangeCurrentValue(effect.amount);
        }
        
    }

}
