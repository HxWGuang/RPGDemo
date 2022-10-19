using System;
using RPG.Core;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class BasicStats : MonoBehaviour, ISaveable
    {
        /// <summary>
        /// 角色开始时的等级
        /// </summary>
        [Range(1,100)]
        [SerializeField] private int startingLevel = 1;

        /// <summary>
        /// 角色类别
        /// </summary>
        [SerializeField] private CharacterEnum characterClass;

        /// <summary>
        /// 存储成长数值的类
        /// </summary>
        [SerializeField] private Progression progression = null;

        /// <summary>
        /// 升级时的粒子效果
        /// </summary>
        [SerializeField] private GameObject levelUpParticleEffect = null;
        
        public event Action OnLevelUp;

        private Experience experience = null;

        private int currentLevel = -1;
        private float expToNextLevel = -1;  //当前等级升到下一级所需要的经验

        public int CurrentLevel => currentLevel;
        public float ExpToNextLevel => expToNextLevel;

        [System.Serializable]
        struct SaveInfo
        {
            public int level;
            public float expToNextLevel;
        }

        private void Awake()
        {

            // Debug.Log("basicstat Awake " + gameObject + " - " + gameObject.GetHashCode());
            
            // Note:为什么放到Awake里面来？
            //因为我想在其他START方法可能调用这个
            //BaseStats时，能够依赖这个状态的设置。
            experience = GetComponent<Experience>();

            // Debug.Log(gameObject.GetHashCode() + "执行Awake()");
            
            if (currentLevel < 1)
            {   // 如果不是从存档中加载的数据则执行下面代码进行初始化
                // currentLevel = 1;
                currentLevel = startingLevel;
            }

            if (experience != null && expToNextLevel < 0)
            { 
                expToNextLevel = GetStat(StatEnum.ExpToLevelUp);
            }

            // Debug.Log(gameObject.GetHashCode() + "当前等级：" + currentLevel);
        }

        private void OnEnable()
        {
            // 订阅获取经验事件
            
            if (experience != null) //玩家才需要进行下面的操作
            {
                experience.OnExpGained += CheckShouldLevelUp;
            }
        }

        private void OnDisable()
        {
            // 取消订阅获取经验事件（防止出bug）
            
            if (experience != null) //玩家才需要进行下面的操作
            {
                experience.OnExpGained -= CheckShouldLevelUp;
            }
        }

        // private void Start()
        // {
        //     // Debug.Log("执行BasicStats - Start()");
        //     
        //     if (currentLevel < 1)
        //     {   // 如果不是从存档中加载的数据则执行下面代码进行初始化
        //         // currentLevel = 1;
        //         currentLevel = startingLevel;
        //     }
        //
        //     if (experience != null && expToNextLevel < 0)
        //     { 
        //         expToNextLevel = GetStat(StatEnum.ExpToLevelUp);
        //     }
        // }

        /// <summary>
        /// 每当获取到经验时会触发该事件，用于判断是否该升级
        /// </summary>
        private void CheckShouldLevelUp()
        {
            // Debug.Log("basicstat CheckShouldLevelUp " + gameObject + " - " + gameObject.GetHashCode());
            float expRemain;
            do
            {
                expRemain = experience.CurrentExp - expToNextLevel;
                if (expRemain >= 0)   //可以升级
                {
                    currentLevel++;
                    
                    // 升级时的粒子效果
                    LevelUpEffect();
                    // 发布升级消息
                    OnLevelUp();
                    
                    experience.CurrentExp = expRemain;
                    expToNextLevel = GetStat(StatEnum.ExpToLevelUp);
                }
            } while (expRemain >= 0);
        }

        /// <summary>
        /// 获取默认设置等级(1级)下的状态信息
        /// </summary>
        /// <param name="stat">要获取的状态类别</param>
        /// <returns>数值</returns>
        public float GetStat(StatEnum stat)
        {
            // Debug.Log(gameObject.GetHashCode() + "查询当前等级：" + currentLevel);
            // return progression.GetStat(stat ,characterClass, currentLevel);
            return (GetBaseStat(stat, currentLevel) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat));
        }

        public float GetBaseStat(StatEnum stat, int level)
        {
            return progression.GetStat(stat ,characterClass, level);
        }

        /// <summary>
        /// 对指定的状态进行一个综合计算，把所有对这个状态的固定数值增益与
        /// 负增益都计算进去
        /// </summary>
        /// <param name="stat">指定的状态</param>
        /// <returns>综合计算后的结果</returns>
        private float GetAdditiveModifiers(StatEnum stat)
        {
            float sum = 0;
            foreach (var modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in modifierProvider.GetAdditiveModifiers(stat))
                {
                    sum += modifier;
                }
            }
            return sum;
        }

        /// <summary>
        /// 获取所有附加在角色身上的百分比增益
        /// </summary>
        /// <param name="stat">状态</param>
        /// <returns>返回结果</returns>
        private float GetPercentageModifier(StatEnum stat)
        {
            float total = 0;
            foreach (var modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in modifierProvider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            
            return total / 100;
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        #region 保存操作

        public object CaptureState()
        {
            SaveInfo info;
            info.level = currentLevel;
            info.expToNextLevel = expToNextLevel;
            return info;
        }

        public void RestoreState(object state)
        {
            // Debug.Log("执行RestoreState");
            
            SaveInfo savedInfo = (SaveInfo) state;
            currentLevel = savedInfo.level;
            expToNextLevel = savedInfo.expToNextLevel;
        }

        #endregion
    }
}
