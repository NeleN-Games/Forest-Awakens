using System.Collections.Generic;
using Databases;
using DG.Tweening;
using Enums;
using Hud.Slots;
using Interfaces;
using Services;
using TMPro;
using UnityEngine;

namespace Hud
{
    public class InventoryUI : MonoBehaviour,IInitializable
    {
        public RectTransform panel;
        public GameObject slotPrefab;
        public Transform slotParent;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private SourceDatabase sourceDatabase;

        private readonly Dictionary<SourceType, InventorySlotUI> _slots = new();

        private bool _isOpen = false;
        private  Vector2 _cashedPanelPosition; 
        public void Initialize()
        {
            sourceDatabase=ServiceLocator.Get<SourceDatabase>();
            RefreshInventory(ServiceLocator.Get<PlayerInventory>().GetInventory());
            panel.anchoredPosition = new Vector2(-panel.rect.width, 0);
            _cashedPanelPosition = new Vector2(-panel.rect.width, 0);
        }

        public void OnDestroy() { }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }

        private void ToggleInventory()
        {
            _isOpen = !_isOpen;
            if (_isOpen)
                panel.DOAnchorPos(-_cashedPanelPosition/2, 0.5f).SetEase(Ease.OutExpo);
            else
                panel.DOAnchorPos(_cashedPanelPosition, 0.5f).SetEase(Ease.InExpo);
        }

        public void RefreshInventory(Dictionary<SourceType, int> inventoryData)
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);
        
            _slots.Clear();

            foreach (var item in inventoryData)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotParent);
                var slot = slotObj.GetComponent<InventorySlotUI>();
                slot.Setup(item.Key, item.Value);
                _slots[item.Key] = slot;
            }
        }

        public void ShowTooltip(string text, Vector2 position)
        {
            tooltipText.text = text;
            tooltipText.transform.position = position;
            tooltipText.gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            tooltipText.gameObject.SetActive(false);
        }

       
    }
}
