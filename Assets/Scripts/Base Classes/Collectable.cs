using System;
using Enums;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class Collectable : MonoBehaviour
{
   [SerializeField]
   protected ItemType itemType;
   [SerializeField]
   protected bool isTriggerable;
   private bool _isCollected;

   public virtual void OnCollect(GameObject collector)
   {
      collector.TryGetComponent(out PlayerInventory playerInventory);
      if (_isCollected) return;
      
      _isCollected = true;
      Destroy(gameObject);
   }

   private void Awake()
   {
      gameObject.tag = "Collectable";
      GetComponent<Collider2D>().isTrigger = isTriggerable;
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
}
