using UnityEngine;
using GameDevTV.Inventories;
using GameDevTV.Core.UI.Dragging;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// 用于显示槽中的物品和它的相关信息。物品信息的显示与数据存储分离。
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [Tooltip("物品icon")]
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;              // 该物品槽下标
        InventoryItem item;     // 物品的InventoryItem类
        Inventory inventory;    // 玩家的背包

        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }
    }
}