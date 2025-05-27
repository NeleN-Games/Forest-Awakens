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
      private bool _isCollected;
      private Collider2D _collider;
      public virtual void OnCollect(GameObject collector)
      {
         collector.TryGetComponent(out PlayerInventory playerInventory);
         if (_isCollected) return;
         playerInventory.AddItem(itemType);
         _isCollected = true;
         Destroy(gameObject);
      }

      private void Awake()
      {
         gameObject.tag = "Collectable";
         _collider = GetComponent<Collider2D>();
         _collider.isTrigger = isTriggerable;
      }

      private void OnEnable()
      {
         _isCollected = false;
      }

      private void OnDisable()
      {
         _isCollected = true;
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
