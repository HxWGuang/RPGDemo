using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BasicStats _basicStats = null;
        private TextMeshProUGUI _levelValue = null;

        void Awake()
        {
            _basicStats = GameObject.FindWithTag("Player").GetComponent<BasicStats>();
            _levelValue = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            _levelValue.text = String.Format("{0:0}", _basicStats.CurrentLevel);
        }
    }
}
