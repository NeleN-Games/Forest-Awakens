using Databases;
using Enums;
using Services;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hud.Slots
{
    public class InventorySlotUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public Image icon;
        public TextMeshProUGUI countText;

        private SourceType _sourceType;

        public void Setup(SourceType type, int count)
        {
            _sourceType = type;
            icon.sprite = ServiceLocator.Get<SourceDatabase>().Get(type).icon;
            countText.text = count.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryUI.Instance.ShowTooltip(ServiceLocator.Get<SourceDatabase>().Get(_sourceType).enumType.ToString(), Input.mousePosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryUI.Instance.HideTooltip();
        }
    }
}
