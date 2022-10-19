using System;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// 可以放置快捷道具栏并使用的背包物品
    /// </summary>
    /// <remarks>
    /// 该类应该用作该类型物品的基类，子类应该实现"Use"方法。
    /// </remarks>
    [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Action Item"))]
    public class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("该物品每次使用时是否会消耗它的实例")]
        [SerializeField] bool consumable = false;

        // PUBLIC

        /// <summary>
        /// 触发物品的使用，重载该方法来提供对应功能
        /// </summary>
        /// <param name="user">使用这个动作的角色</param>
        public virtual void Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
        }

        public bool isConsumable()
        {
            return consumable;
        }
    }
}