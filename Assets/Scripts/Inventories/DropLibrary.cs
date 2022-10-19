using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        // CONFIG DATA
        
        // - Drop Chance
        // - Min drops
        // - Max drops
        // - Potential Drops
        //   - Relative chance
        //   - Min items
        //   - Max items

        [Tooltip("所有可能的掉落物品")]
        [SerializeField] private DropConfig[] potentialDrops;
        [Tooltip("触发掉落的几率")]
        [Range(0,100)]
        [SerializeField] private float[] dropChancePercentage;
        [Tooltip("一次掉落中物品种类最小值")]
        [SerializeField] private int[] minDrops;
        [Tooltip("一次掉落中物品种类最大值")]
        [SerializeField] private int[] maxDrops;

        [System.Serializable]
        class DropConfig
        {
            [Tooltip("物品")]
            public InventoryItem item;
            [Tooltip("相对掉落该物品的几率")]
            public float[] relativeChance;
            [Tooltip("该物品最小掉落数（只对可堆叠物品生效）")]
            public int[] minNumber;
            [Tooltip("该物品最大掉落数（只对可堆叠物品生效）")]
            public int[] maxNumber;

            /// <summary>
            /// 计算该掉落品的掉落数量，如果为不可堆叠物品，则始终返回1
            /// </summary>
            /// <param name="level">等级</param>
            /// <returns>计算结果</returns>
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }

                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }
        
        // STATE DATA

        /// <summary>
        ///  掉落物品结构体，掉落物的情况存储在这里面
        /// </summary>
        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }
        
        // PUBLIC
        
        /// <summary>
        /// 利用迭代器返回所有的掉落物
        /// </summary>
        /// <param name="level">当前角色等级</param>
        /// <returns>返回该迭代器</returns>
        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            // 首先判断这次击杀是否会掉落
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            // 根据随机出来的掉落物品数量依次获取掉落物
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }
        
        // PRIVATE

        /// <summary>
        /// 计算是否会触发掉落机制
        /// </summary>
        /// <param name="level">等级</param>
        /// <returns>计算结果</returns>
        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        /// <summary>
        /// 计算物品随机掉落数（针对可堆叠物品）
        /// </summary>
        /// <param name="level">角色当前等级</param>
        /// <returns>计算结果</returns>
        private int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return Random.Range(min, max + 1);
        }

        /// <summary>
        /// 根据当前角色等级和当前怪物所有可掉落物品中随机一个掉落物品
        /// </summary>
        /// <param name="level">当前角色等级</param>
        /// <returns>返回掉落物结构体</returns>
        private Dropped GetRandomDrop(int level)
        {
            var drop = SelectRandomItem(level); // 随机掉落物
            var result = new Dropped(); // 创造掉落物结构体，准备存储
            
            result.item = drop.item;
            result.number = drop.GetRandomNumber(level);
            return result;
        }

        /// <summary>
        /// 在所有可掉落物品中随机选择掉落物品
        /// </summary>
        /// <param name="level">当前角色等级</param>
        /// <returns>返回结果</returns>
        private DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);  // 获得所有可掉落物的权重之和
            float randomRoll = Random.Range(0, totalChance);    // 随机一个权重数值
            float chanceTotal = 0;
            
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (chanceTotal >= randomRoll)   // 疑问：如果chanceTotal = randomRoll呢？如果没有等于号的话，当randomRoll
                {                                // 等于最大值totalChance的时候就会返回null容易出bug
                    return drop;
                }
            }

            return null;
        }

        /// <summary>
        /// 计算总的权重。根据当前角色的等级计算所有可掉落物品的当前权重之和。
        /// </summary>
        /// <param name="level">角色等级</param>
        /// <returns>返回结果</returns>
        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance, level);
            }

            return total;
        }

        /// <summary>
        /// 根据等级来获取数值
        /// </summary>
        /// <param name="values">存储每个等级对应数值的数组</param>
        /// <param name="level">指定的等级</param>
        /// <typeparam name="T">传入的数组类型</typeparam>
        /// <returns>返回的结果</returns>
        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }

            if (level > values.Length)
            {
                return values[values.Length - 1];
            }

            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }
    }
}