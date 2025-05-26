using UnityEngine;

namespace Models
{
   public class Rock : Collectable
   {
      public override void OnCollect(GameObject collector)
      { 
         collector.TryGetComponent(out PlayerInventory playerInventory);
         playerInventory.AddItem(itemType); 
         Destroy(gameObject);
      }
   }
}
