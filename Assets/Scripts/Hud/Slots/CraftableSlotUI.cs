using System;
using Enums;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hud.Slots
{
    public class CraftableSlotUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        private Action onClicked;
        private ICraftable _craftable;

        public void Setup(ICraftable craftable)
        {
            _craftable = craftable;
            title.text = _craftable.UniqueId.UniqueName;
            onClicked += _craftable.Craft;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClicked?.Invoke();
        }

        public void ChangeAvailability(bool isAvailable)
        {
            if (isAvailable)
            {
                ChangeToAvailable();
            }
            else
            {
                ChangeToUnavailable();
            }
        }

        private void ChangeToAvailable()
        {
            
        }
        private void ChangeToUnavailable()
        {
            
        }
    }
}
