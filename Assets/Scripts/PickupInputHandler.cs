using Base_Classes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInventory inventory;
    public float pickupRange = 1.5f;
    private const string CollectableTag = "Collectable";
    private readonly Collider2D[] _results = new Collider2D[5]; 

    private bool _isPickupHeld;
    private HarvestableSource currentHarvestableSource;
    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isPickupHeld = true;
        }
        else if (context.canceled)
        {
            _isPickupHeld = false;
        }
    }
    private void Update()
    {
        if (_isPickupHeld)
        {
            CheckPickup();
        }
        else if (currentHarvestableSource is not null)
        {
            currentHarvestableSource.RequestStopCollecting();
        }
    }
    private void CheckPickup()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRange, _results);

        for (int i = 0; i < size; i++)
        {
            var hit = _results[i];
            if (!hit.CompareTag(CollectableTag)) continue;
            
            if (!hit.TryGetComponent<Collectable>(out var collectable)) continue;
            
            if (collectable is HarvestableSource sourceCollectable)
            {
                currentHarvestableSource = sourceCollectable;
            }
            else
            {
                currentHarvestableSource = null;
            }
            collectable.OnCollect(gameObject);
            return;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        bool hasCollectableNearby = false;

        if (Application.isPlaying && enabled)
        {
            int size = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRange, _results);
            for (int i = 0; i < size; i++)
            {
                if (!_results[i].CompareTag(CollectableTag)) continue;
                hasCollectableNearby = true;
                break;
            }
        }

        Gizmos.color = hasCollectableNearby ?
            (currentHarvestableSource?Color.blue :Color.green )
            : Color.white;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

}
