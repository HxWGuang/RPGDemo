﻿using UnityEngine;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// An slot for the players equipment.
    /// 装备槽
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

        // CACHE
        Equipment equipInst;

        // LIFECYCLE METHODS

        #region sys

        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            equipInst = player.GetComponent<Equipment>();
            equipInst.equipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }

        #endregion
        
        // PUBLIC

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            equipInst.AddItem(equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return equipInst.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            equipInst.RemoveItem(equipLocation);
        }

        // PRIVATE

        void RedrawUI()
        {
            icon.SetItem(equipInst.GetItemInSlot(equipLocation));
        }
    }
}