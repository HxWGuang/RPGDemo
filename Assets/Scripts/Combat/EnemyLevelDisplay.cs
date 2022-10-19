using System;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class EnemyLevelDisplay : MonoBehaviour
    {
        private Fighter _fighter = null;
        private TextMeshProUGUI _enemyLevelValue = null;

        void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _enemyLevelValue = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            if (_fighter.Target != null)
            {
                _enemyLevelValue.text = String.Format("{0:0}",
                    _fighter.Target.gameObject.GetComponent<BasicStats>().CurrentLevel);
            }
            else
            {
                _enemyLevelValue.text = "N/A";
            }
        }
    }
}
