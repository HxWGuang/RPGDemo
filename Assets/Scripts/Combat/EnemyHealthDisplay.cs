using System;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        // private GameObject _player;
        // private Health _target = null;

        private Fighter _fighter = null;
        private TextMeshProUGUI _healthValue = null;

        void Awake()
        {
            // _player = GameObject.FindWithTag("Player");
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _healthValue = GetComponent<TextMeshProUGUI>();
        }
        
        void Update()
        {
            // _target = _player.GetComponent<Fighter>().Target;
            // _healthValue.text = $"{_health.GetHealthPercent()}:0.0";
            if (_fighter.Target != null)
            {
                _healthValue.text =
                    String.Format("{0:0} / {1:0}", _fighter.Target.HealthPoint, _fighter.Target.MaxHealthPoint);
            }
            else
            {
                _healthValue.text = "N/A";
            }
        }
    }
}
