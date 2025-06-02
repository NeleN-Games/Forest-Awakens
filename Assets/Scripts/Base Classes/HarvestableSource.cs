using Databases;
using DG.Tweening;
using Enums;
using Models;
using Services;
using UnityEngine;

namespace Base_Classes
{
    /// <summary>
    /// Handles the logic for collecting a resource, spawning output, and managing visual state based on health.
    /// </summary>
    [RequireComponent(typeof(ResourceCapacity))]
    public class HarvestableSource : Collectable
    {
        /// <summary>
        /// Total time it takes to fully collect the resource.
        /// </summary>
        [SerializeField] private float fullTimeToCollect = 3;

        /// <summary>
        /// Total number of resource items that can be extracted.
        /// </summary>
        [SerializeField] private int sourceAmount = 5;

        private GameObject _resourcePrefab;

        /// <summary>
        /// Gets the prefab associated with the resource item.
        /// </summary>
        private GameObject ResourcePrefab
        {
            get
            {
                if (_resourcePrefab == null)
                {
                    _resourcePrefab = ServiceLocator.Get<SourceDatabase>().Get(itemType).prefab;
                }
                return _resourcePrefab;
            }
        }

        private float _sourceExtractSteps;
        private int _producedCount;

        /// <summary>
        /// Indicates if the resource is currently being collected.
        /// </summary>
        public bool IsCollecting { get; private set; }

        private Vector2 _randomPosition;
        private ResourceCapacity resourceCapacity;

        /// <summary>
        /// Called when the object is initialized. Sets extract ratios and hooks events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            resourceCapacity = GetComponent<ResourceCapacity>();
            resourceCapacity.OnDepleted += FinalizeCollection;
            SetExtractRatios();
        }

        /// <summary>
        /// Calculates the extraction step and configures the SourceHealth accordingly.
        /// </summary>
        private void SetExtractRatios()
        {
            var maxHealth = resourceCapacity.MaxHealth;
            _sourceExtractSteps = maxHealth / sourceAmount;

            resourceCapacity.SetDecreaseRatio(fullTimeToCollect);
            resourceCapacity.SetExtractionParameters(_sourceExtractSteps, sourceAmount);
        }

        /// <summary>
        /// Called when the object is enabled. Resets collection state.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            IsCollected = false;
        }

        /// <summary>
        /// Called when the object is disabled. Marks the resource as collected.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            IsCollected = true;
        }

        /// <summary>
        /// Handles player interaction with the collectable resource.
        /// </summary>
        /// <param name="collector">The object attempting to collect the resource.</param>
        public override void OnCollect(GameObject collector)
        {
            if (IsCollected) return;

            IsCollecting = true;
            resourceCapacity.DecreaseHealth(_producedCount);
        }

        /// <summary>
        /// Finalizes the collection process and destroys the resource object.
        /// </summary>
        private void FinalizeCollection()
        {
            IsCollecting = false;
            IsCollected = true;
            Destroy(gameObject);
        }

        /// <summary>
        /// Spawns a resource item at a random offset and increments the produced count.
        /// </summary>
        public void CheckExtractSource()
        {
            if (ResourcePrefab != null)
            {
                _randomPosition = Random.insideUnitCircle * 3;
                Instantiate(ResourcePrefab, (Vector2)transform.position + _randomPosition, Quaternion.identity);
                _producedCount++;
            }
            else
            {
                Debug.LogWarning("No resource prefab assigned");
            }
        }

        /// <summary>
        /// Stops the current collection process.
        /// </summary>
        protected virtual void StopCollecting()
        {
            IsCollecting = false;
        }

        /// <summary>
        /// Requests the system to stop collecting the resource.
        /// </summary>
        public void RequestStopCollecting()
        {
            StopCollecting();
        }

        /// <summary>
        /// Requests a visual update based on the resource's health state.
        /// </summary>
        /// <param name="state">The new health state.</param>
        public void RequestUpdateVisualState(HealthState state)
        {
            switch (state)
            {
                case HealthState.Low:
                    UpdateVisualLowState();
                    break;
                case HealthState.Medium:
                    UpdateVisualMediumState();
                    break;
            }
        }

        /// <summary>
        /// Visually represents a low health state (e.g., by shrinking the object).
        /// </summary>
        protected virtual void UpdateVisualLowState()
        {
            transform.DOScale(0.4f, 0.3f);
        }

        /// <summary>
        /// Visually represents a medium health state (e.g., by partially shrinking the object).
        /// </summary>
        protected virtual void UpdateVisualMediumState()
        {
            transform.DOScale(0.7f, 0.3f);
        }
    }
}
