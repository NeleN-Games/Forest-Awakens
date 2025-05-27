using Base_Classes;
using UnityEngine;
using UnityEngine.Serialization;

public class PickupInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInventory inventory;
    public float pickupRange = 1.5f;
    private const string CollectableTag = "Collectable";
    private readonly Collider2D[] _results = new Collider2D[10]; 

    public void OnPickup()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRange, _results);

        for (int i = 0; i < size; i++)
        {
            var hit = _results[i];
            if (!hit.CompareTag(CollectableTag)) continue;

            var collectable = hit.GetComponent<Collectable>();
            if (collectable == null) continue;
            
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
                if (!_results[i].CompareTag(CollectableTag)) continue;
                hasCollectableNearby = true;
                break;
            }
        }

        Gizmos.color = hasCollectableNearby ? Color.green : Color.white;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

}
