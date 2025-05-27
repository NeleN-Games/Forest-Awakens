using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    private ItemType _itemType;

    public void Setup(ItemType type, int count)
    {
        _itemType = type;
        icon.sprite = ItemDatabase.GetItem(type).icon;
        icon.color = ItemDatabase.GetItem(type).color;
        countText.text = count.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUI.Instance.ShowTooltip(ItemDatabase.GetItem(_itemType).itemName, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideTooltip();
    }
}
