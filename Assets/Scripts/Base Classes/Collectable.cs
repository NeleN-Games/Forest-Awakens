using System;
using Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base_Classes
{
   [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
   public abstract class Collectable : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
   {
      [SerializeField]
      protected ItemType itemType;
      
      [SerializeField]
      protected bool isTriggerable;
      protected bool IsCollected;
      private Collider2D _collider;
      public virtual void OnCollect(GameObject collector)
      {
         collector.TryGetComponent(out PlayerInventory playerInventory);
         if (IsCollected) return;
         playerInventory.AddItem(itemType);
         IsCollected = true;
         Destroy(gameObject);
      }

      protected virtual void Awake()
      {
         gameObject.tag = "Collectable";
         _collider = GetComponent<Collider2D>();
         _collider.isTrigger = isTriggerable;
      }

      protected virtual void OnEnable()
      {
         IsCollected = false;
      }

      protected virtual void OnDisable()
      {
         IsCollected = true;
      }

      public ItemType GetItemType()
      {
         return itemType;
      }

      public void OnPointerEnter(PointerEventData eventData)
      {
         InteractionManager.Instance.ShowTooltip(itemType.ToString(),
            transform.position + Vector3.up * _collider.bounds.size.y);
      }

      public void OnPointerExit(PointerEventData eventData)
      {
         InteractionManager.Instance.HideTooltip();
      }
   }
}
