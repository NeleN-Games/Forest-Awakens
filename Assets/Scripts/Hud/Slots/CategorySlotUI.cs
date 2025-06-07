using System;
using System.Collections.Generic;
using Enums;
using Models.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hud.Slots
{
    public class CategorySlotUI : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI title;
        public Action<CategorySlotUI> OnClicked;
        public CategoryType Category { get; private set; }

        public void Setup(CategoryType category)
        {
            Category=category;
            title.text = category.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
    }
}
