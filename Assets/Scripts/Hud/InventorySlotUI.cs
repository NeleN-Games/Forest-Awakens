using Databases;
using Enums;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hud
{
    public class InventorySlotUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public Image icon;
        public TextMeshProUGUI countText;

        private SourceType _sourceType;

        public void Setup(SourceType type, int count)
        {
            _sourceType = type;
            icon.sprite = SourceDatabase.Get(type).icon;
            countText.text = count.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryUI.Instance.ShowTooltip(SourceDatabase.Get(_sourceType).type.ToString(), Input.mousePosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryUI.Instance.HideTooltip();
        }
    }
}
