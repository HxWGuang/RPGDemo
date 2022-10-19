using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;

namespace RPG.Inventories
{
    public class StatEquipment : Equipment, IModifierProvider
    {
        /// <summary>
        /// 获取装备栏上所有装备对属性的固定数值增益
        /// </summary>
        /// <param name="stat">属性</param>
        /// <returns>返回结果</returns>
        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatEnum stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                
                foreach (var modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        /// <summary>
        /// 获取装备栏上所有装备对属性的百分比数值增益
        /// </summary>
        /// <param name="stat">属性</param>
        /// <returns>返回结果</returns>
        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatEnum stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (var modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}