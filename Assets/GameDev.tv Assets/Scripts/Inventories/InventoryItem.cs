using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// 用ScriptableObject来存储任意可以放进背包的物品
    /// </summary>
    /// <remarks>
    /// 在练习中，通常使用该类的子类，例如"ActionItem"、"EquipableItem"
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        // CONFIG DATA
        [Tooltip("自动生成UUID用来保存和加载。如果想重新生成一个UUID就清除该字段。")]
        [SerializeField] string itemID = null;
        [Tooltip("在UI上显示的物品名字")]
        [SerializeField] string displayName = null;
        [Tooltip("在UI上显示的物品信息")]
        [SerializeField][TextArea] string description = null;
        [Tooltip("在背包中展示的物品icon")]
        [SerializeField] Sprite icon = null;
        [Tooltip("该物品掉落时所需要生成的pickup预制体")]
        [SerializeField] Pickup pickup = null;
        [Tooltip("如果为true，则多个相同的物品将会堆叠在同一个物品槽中")]
        [SerializeField] bool stackable = false;

        // STATE
        static Dictionary<string, InventoryItem> itemLookupCache;   // 物品查询缓存（静态对象）

        // PUBLIC

        /// <summary>
        /// 通过UUID获取该物品实例
        /// </summary>
        /// <param name="itemID">
        /// 在游戏实例中持续存在的UUID
        /// </param>
        /// <returns>
        /// 与该ID绑定的背包物品实例
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }
        
        /// <summary>
        /// 在世界中生成pickup游戏对象
        /// </summary>
        /// <param name="position">生成pickup的位置</param>
        /// <param name="number">该pickup包含多少个物品实例？</param>
        /// <returns>生成的pickup对象引用</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        /// <summary>
        /// 获取该物品的icon
        /// </summary>
        /// <returns>物品icon精灵图</returns>
        public Sprite GetIcon()
        {
            return icon;
        }

        /// <summary>
        /// 获取物品ID
        /// </summary>
        /// <returns>物品ID字符串</returns>
        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        /// <summary>
        /// 获取物品显示的名字
        /// </summary>
        /// <returns>物品名字符串</returns>
        public string GetDisplayName()
        {
            return displayName;
        }

        /// <summary>
        /// 获取物品描述
        /// </summary>
        /// <returns>物品描述字符串</returns>
        public string GetDescription()
        {
            return description;
        }

        // PRIVATE
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // 如果为空白则生成并保存一个新的UUID
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // ISerializationCallbackReceiver接口要求实现这个方法
            // 但是我们不需要做任何事，写个空实现就行了
        }
    }
}
