using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// 包含每个角色的成长数值
    /// 角色类别：
    ///     状态类别：
    ///         等级：
    /// </summary>
    [CreateAssetMenu(fileName = "Progression",menuName = "RPG/Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        // CONFIG DATA
        
        [Tooltip("所有角色的成长数值数组")]
        [SerializeField] private ProgressionCharacterClass[] progressionCharacterClasses = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [Tooltip("角色类型")]
            public CharacterEnum characterClass;    // 角色类别
            [Tooltip("属性类型")]
            public ProgressionStat[] stats;     // 属性数组
        }

        [System.Serializable]
        class ProgressionStat
        {
            [Tooltip("属性")]
            public StatEnum stat;   // 属性
            [Tooltip("等级数组（每个等级对应的该属性的数值）")]
            public float[] levels;  // 等级
            // public float[] basicHealth;
            // public float[] basicAttack;
            // public int[] rewardExp;
        }
        
        // DATA CACHE
        
        private Dictionary<CharacterEnum, Dictionary<StatEnum, float[]>> lookupTable = null;

        // PUBLIC
        
        /// <summary>
        /// 从GetHealth改名为GetStat，更为通用的名字
        /// </summary>
        /// <param name="stat">要查询的状态</param>
        /// <param name="characterClass">要查询的角色的类别</param>
        /// <param name="level">要查询的角色的等级</param>
        /// <returns>返回数值</returns>
        public float GetStat(StatEnum stat ,CharacterEnum characterClass, int level)
        {
            // for (int i = 0; i < progressionCharacterClasses.Length; i++)
            // {
            //     if (progressionCharacterClasses[i].characterClass == characterClass)
            //     {
            //         for (int j = 0; j < progressionCharacterClasses[i].stats[j].stat)
            //         // return progressionCharacterClasses[i].basicHealth[level - 1];
            //     }
            // }
            // return 0;

            // foreach (ProgressionCharacterClass progressionCharacterClass in progressionCharacterClasses)
            // {
            //     if (progressionCharacterClass.characterClass != characterClass) continue;
            //     foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
            //     {
            //         if (progressionStat.stat != stat) continue;
            //         if (progressionStat.levels.Length < level) continue;
            //         return progressionStat.levels[level - 1];
            //     }
            // }
            //
            // return 0;
            
            CreateLookupTable();
            
            // foreach (var (c, statsTable) in lookupTable)
            // {
            //     if (c != characterClass) continue;
            //
            //     foreach (var keyValuePair in statsTable)
            //     {
            //         if (keyValuePair.Key != stat) continue;
            //         
            //         if (keyValuePair.Value.Length < level) continue;
            //
            //         return keyValuePair.Value[level - 1];
            //     }
            // }
            
            //我人麻了，这么简单的事情居然写个双重循环，字典是可以像数组
            //一样直接用下标访问的，不用再用循环遍历字典。。。
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length < level ? 0 : levels[level - 1];
        }

        // PRIVATE
        
        private void CreateLookupTable()
        {
            // 判断是否为空，为空说明是第一次加载查询，往下执行从文件中读取数据存储
            // 到lookupTable中用于缓存
            if (lookupTable != null) return;
            
            lookupTable = new Dictionary<CharacterEnum, Dictionary<StatEnum, float[]>>();
            
            foreach (var progressionCharacterClass in progressionCharacterClasses)
            {
                var statsTable = new Dictionary<StatEnum, float[]>();

                foreach (var progressionStat in progressionCharacterClass.stats)
                {
                    statsTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionCharacterClass.characterClass] = statsTable;
            }
        }
    }
}