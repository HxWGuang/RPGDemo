using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// 该脚本挂载在Pickup预制体上。
    /// 包含掉落物的物品种类和数量等数据。
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        InventoryItem item; // 该Pickup中的物品
        int number = 1; // 物品数量

        // CACHED REFERENCE
        Inventory inventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        // PUBLIC

        /// <summary>
        /// 创建预制体后设置关键信息
        /// </summary>
        /// <param name="item">该预制体所包含的物品的物品种类</param>
        /// <param name="number">该物品的数量</param>
        public void Setup(InventoryItem item, int number)
        {
            this.item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            this.number = number;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }

        /// <summary>
        /// 执行捡起操作时调用这个方法
        /// </summary>
        public void PickupItem()
        {
            // 查找玩家背包是否还有空格子
            bool foundSlot = inventory.AddToFirstEmptySlot(item, number);
            
            // 如果有空间则存入背包并销毁该Pickup预制体
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 判断是否能够捡起该物品
        /// </summary>
        /// <returns>返回结果</returns>
        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }
    }
}