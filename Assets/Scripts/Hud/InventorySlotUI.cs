using Enums;
using Models.Scriptable_Objects;
using TMPro;
using UI;
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
        icon.sprite = SourceDatabase.GetSource(type).icon;
        icon.color = SourceDatabase.GetSource(type).color;
        countText.text = count.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUI.Instance.ShowTooltip(SourceDatabase.GetSource(_itemType).itemName, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideTooltip();
    }
}
