using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// 具有属性加成的装备
    /// </summary>
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipeable Item"))]
    public class StatEquipableItem : EquipableItem,IModifierProvider
    {
        // DATA

        [Tooltip("该装备所有对属性的固定数值增益")]
        [SerializeField] private Modifier[] additiveModifiers;
        [Tooltip("该装备所有对属性的百分比数值增益")]
        [SerializeField] private Modifier[] percentageModifiers;
            
        // PRIVATE
        
        /// <summary>
        /// 存储属性的增益值
        /// </summary>
        [System.Serializable]
        struct Modifier
        {
            public StatEnum stat;
            public float value;
        }
        
        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatEnum stat)
        {
            foreach (Modifier modifier in additiveModifiers)
            {
                if (stat == modifier.stat)
                {
                    yield return modifier.value;
                }
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatEnum stat)
        {
            // 遍历百分比增益数组
            foreach (Modifier modifier in percentageModifiers)
            {
                // 如果查询的stat等于存储的stat则把值返回
                if (stat == modifier.stat)
                {
                    yield return modifier.value;
                }
            }
        }
    }
}