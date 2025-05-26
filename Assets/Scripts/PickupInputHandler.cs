using UnityEngine;

public class PickupInputHandler : MonoBehaviour
{
    private PlayerInventory _inventory;
    public float pickupRange = 1.5f;
    public string collectableTag = "Collectable";
    private readonly Collider2D[] _results = new Collider2D[10]; 
    private void Awake()
    {
        _inventory=PlayerInventory.Instance;
    }

    public void OnPickup()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRange, _results);

        for (int i = 0; i < size; i++)
        {
            var hit = _results[i];
            if (!hit.CompareTag(collectableTag)) continue;

            var collectable = hit.GetComponent<Collectable>();
            if (collectable == null) continue;

            _inventory.AddItem(collectable.GetItemType());
            collectable.OnCollect(gameObject);
            Debug.Log("Picked up " + collectable.GetItemType());
            return;
        }
        Debug.LogWarning("No collectable item found");
    }
    private void OnDrawGizmosSelected()
    {
        bool hasCollectableNearby = false;

        if (Application.isPlaying && enabled)
        {
            int size = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRange, _results);
            for (int i = 0; i < size; i++)
            {
                if (!_results[i].CompareTag(collectableTag)) continue;
                hasCollectableNearby = true;
                break;
            }
        }

        Gizmos.color = hasCollectableNearby ? Color.green : Color.white;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

}
