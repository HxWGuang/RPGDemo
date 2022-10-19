using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private BasicStats _basicStats = null;
        private Experience _experience = null;
        private TextMeshProUGUI _expValue = null;

        void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            _basicStats = GameObject.FindWithTag("Player").GetComponent<BasicStats>();
            _expValue = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            _expValue.text = String.Format("{0:0} / {1:0}", _experience.CurrentExp,
                _basicStats.ExpToNextLevel);
        }
    }
}
