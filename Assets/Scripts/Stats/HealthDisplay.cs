using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health = null;
        private TextMeshProUGUI _healthValue = null;
        
        void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _healthValue = GetComponent<TextMeshProUGUI>();
        }
        
        void Update()
        {
            // _healthValue.text = $"{_health.GetHealthPercent()}:0.0";
            _healthValue.text = String.Format("{0:0} / {1:0}", _health.HealthPoint, _health.MaxHealthPoint);
        }
    }
}
