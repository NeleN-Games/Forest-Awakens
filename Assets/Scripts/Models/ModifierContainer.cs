using System;
using System.Collections.Generic;
using Managers;
using Models.Scriptable_Objects;
using UnityEngine;

namespace Models
{
    [RequireComponent(typeof(Collider2D))]
    public class ModifierContainer : MonoBehaviour
    {
        public List<StatModifierSo> modifiers = new List<StatModifierSo>();

        public event Action OnApplyModifiers;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnEnable()
        {
            OnApplyModifiers -= ApplyModifiers; 
            OnApplyModifiers += ApplyModifiers;
        }

        private void OnDisable()
        {
            OnApplyModifiers -= ApplyModifiers;
        }

        private void ApplyModifiers()
        {
            foreach(var modifier in modifiers)
            {
                StatManager.Instance.ApplyModifier(modifier);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            OnApplyModifiers?.Invoke();
        }
    }
}
