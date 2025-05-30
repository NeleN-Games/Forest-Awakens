using System;
using Base_Classes;
using Enums;
using UnityEngine;

namespace Models
{
    /// <summary>
    /// Manages the health of a resource and triggers visual updates or events when depleted or health state changes.
    /// </summary>
    [RequireComponent(typeof(HarvestableSource))]
    public class ResourceCapacity : MonoBehaviour
    {
        /// <summary>
        /// Maximum health value for the resource.
        /// </summary>
        [SerializeField] private float maxHealth = 100f;

        /// <summary>
        /// The maximum health value (read-only at runtime).
        /// </summary>
        public float MaxHealth => maxHealth;

        private float _decreaseHealthRate;
        private HarvestableSource harvestableSource;
        private float _sourceExtractSteps;
        private float _sourceAmount;

        /// <summary>
        /// Invoked when the resource health reaches zero.
        /// </summary>
        public event Action OnDepleted;

        private HealthState _healthState;

        /// <summary>
        /// Current health state (Full, Medium, or Low).
        /// Setting this property automatically triggers state change events.
        /// </summary>
        private HealthState HealthState
        {
            get => _healthState;
            set
            {
                if (_healthState == value) return;
                _healthState = value;
                OnHealthStateChanged(value);
            }
        }

        private float _currentHealth;

        /// <summary>
        /// The current health value. Triggers state checks on set.
        /// </summary>
        private float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                var healthPercent = (_currentHealth / maxHealth);
                CheckState(healthPercent);
            }
        }

        /// <summary>
        /// Called when the component is initialized. Sets up default health values.
        /// </summary>
        public void Awake()
        {
            _healthState = HealthState.Full;
            harvestableSource = GetComponent<HarvestableSource>();
            SetHealth();
        }

        /// <summary>
        /// Sets the initial full health value and state.
        /// </summary>
        private void SetHealth()
        {
            _healthState = HealthState.Full;
            CurrentHealth = maxHealth;
        }

        /// <summary>
        /// Decreases health over time during collection and triggers extraction logic.
        /// </summary>
        /// <param name="producedCount">Number of resources already produced.</param>
        public void DecreaseHealth(int producedCount)
        {
            float extractAmount = Time.deltaTime * _decreaseHealthRate;
            CurrentHealth -= extractAmount;
            float nextExtractStep = MaxHealth - (producedCount + 1) * _sourceExtractSteps;

            if (!(CurrentHealth <= nextExtractStep) || producedCount >= _sourceAmount) return;

            harvestableSource.CheckExtractSource();

            if (CurrentHealth <= 0)
            {
                OnDepleted?.Invoke();
            }
        }

        /// <summary>
        /// Sets the rate at which the resource health decreases over time.
        /// </summary>
        /// <param name="fullTimeToCollect">The full time it should take to deplete the resource.</param>
        public void SetDecreaseRatio(float fullTimeToCollect)
        {
            _decreaseHealthRate = maxHealth / fullTimeToCollect;
        }

        /// <summary>
        /// Sets the extraction step and total amount of resource to be extracted.
        /// </summary>
        /// <param name="sourceExtractSteps">The step value between each extraction.</param>
        /// <param name="sourceAmount">The total number of extractable items.</param>
        public void SetExtractionParameters(float sourceExtractSteps, float sourceAmount)
        {
            _sourceExtractSteps = sourceExtractSteps;
            _sourceAmount = sourceAmount;
        }

        /// <summary>
        /// Updates visual state based on current health state.
        /// </summary>
        /// <param name="healthState">The new health state.</param>
        private void OnHealthStateChanged(HealthState healthState)
        {
            harvestableSource.RequestUpdateVisualState(healthState);
        }

        /// <summary>
        /// Checks and updates the visual state based on current health percentage.
        /// </summary>
        /// <param name="healthPercent">Current health as a percentage of max health.</param>
        private void CheckState(float healthPercent)
        {
            if (healthPercent < 0.3f)
            {
                if (HealthState != HealthState.Low)
                    HealthState = HealthState.Low;
            }
            else if (healthPercent < 0.6f)
            {
                if (HealthState != HealthState.Medium)
                    HealthState = HealthState.Medium;
            }
        }
    }
}
