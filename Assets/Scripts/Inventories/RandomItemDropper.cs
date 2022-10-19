using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomItemDropper: ItemDropper
    {
        // CONFIG DATA
        [Tooltip("物品散落距离")]
        [SerializeField] private float scatterDistance = 1;

        [Tooltip("敌人可掉落的物品")]
        [SerializeField] private DropLibrary dropLibrary;

        // [Tooltip("掉落数量")]
        // [SerializeField] private int numberOfDrops = 2;
        
        // CONSTANTS
        private const int ATTENPTS = 30;    // 尝试次数

        // 此方法在敌人死亡时由UnityEvent自动调用
        public void RandomDrop()
        {
            BasicStats basicStats = GetComponent<BasicStats>(); // 获取角色当前状态
            var drops = dropLibrary.GetRandomDrops(basicStats.CurrentLevel);    // 拿到所有掉落物

            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);   // 
            }
            
            // for (int i = 0; i < numberOfDrops; i++)
            // {
            //     var item = dropLibrary[Random.Range(0, dropLibrary.Length)];
            //     DropItem(item, 1);
            // }
        }
        
        /// <summary>
        /// 重载父类中的 GetDropLocation() 方法
        /// </summary>
        /// <returns>返回一个随机位置</returns>
        protected override Vector3 GetDropLocation()
        {
            // 随机掉落尝试30次
            for (int i = 0; i < ATTENPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            
            return transform.position;
        }
    }
}