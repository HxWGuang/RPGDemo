using UnityEngine;
using GameDevTV.Inventories;
using InventoryExample.Control;
using RPG.Control;

namespace InventoryExample.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("鼠标点击掉落物");
                pickup.PickupItem();
            }
            return true;
        }
    }
}