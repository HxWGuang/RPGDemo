using System;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour,ISaveable
    {
        public event Action OnExpGained;
        
        private float currentExp = 0;

        public float CurrentExp
        {
            get => currentExp;
            set => currentExp = value;
        }

        public void GainExp(float expValue)
        {
            currentExp += expValue;

            OnExpGained();
        }

        #region 保存操作

        public object CaptureState()
        {
            return currentExp;
        }

        public void RestoreState(object state)
        {
            currentExp = (float) state;
        }

        #endregion
    }
}
