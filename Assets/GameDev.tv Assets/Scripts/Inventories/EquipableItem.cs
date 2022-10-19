using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// 一个可以装备到玩家身上的背包物品。
    /// 武器可以是这个类的子类。
    /// </summary>
    [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("该物品可以装备到哪个位置")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;

        // PUBLIC

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }
    }
}