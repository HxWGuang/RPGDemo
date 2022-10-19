using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Attack;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            //因为直接就是调用这个脚本里的代码，所以下面的操作都不再需要了
            // CombatTarget target = hit.transform.GetComponent<CombatTarget>();
            // if (target == null) continue;
            Fighter fighter = playerController.GetComponent<Fighter>();
            
            if (!fighter.CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(0))
            {
                fighter.Attack(gameObject);
            }
            
            return true;
        }
    }
}