using System.Collections.Generic;
using DG.Tweening;
using Enums;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        public RectTransform panel;
        public GameObject slotPrefab;
        public Transform slotParent;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private ItemDatabase itemDatabase;

        private readonly Dictionary<ItemType, InventorySlotUI> _slots = new();

        private bool _isOpen = false;
        private  Vector2 _cashedPanelPosition; 
        private void Start()
        {
            panel.anchoredPosition = new Vector2(-panel.rect.width, 0);

            PlayerInventory.Instance.OnInventoryChanged += RefreshInventory;
            RefreshInventory(PlayerInventory.Instance.GetInventory());
            _cashedPanelPosition = new Vector2(-panel.rect.width, 0);
            ItemDatabase.Initialize(itemDatabase);
        }

        private void OnDisable()
        {
            PlayerInventory.Instance.OnInventoryChanged -= RefreshInventory;
        }

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

        private void RefreshInventory(Dictionary<ItemType, int> inventoryData)
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
