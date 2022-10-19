using System.Collections;
using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickups : MonoBehaviour,IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float healValue = 0f;
        [SerializeField] private float hidePickupTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject obj)
        {
            if (weaponConfig != null)
            {
                obj.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }

            if (healValue > 0)
            {
                obj.GetComponent<Health>().Heal(healValue);
            }
            // Destroy(gameObject);
            StartCoroutine(HidePickupAndRespawn(hidePickupTime));
        }


        private IEnumerator HidePickupAndRespawn(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            // GetAllMeshRendersAndCollider(true);

            GetComponent<Collider>().enabled = shouldShow;

            //第一次知道找到对象下的所有孩子还可以这样！！！
            //直接遍历transform就可以了，不用根据transform.childCount来
            //一个个找了，但是这个foreach遍历是比普通for遍历要更
            //消耗资源的，要注意！！！
            foreach (Transform child in transform)
            {
                // Debug.Log(child.name);
                
                // Note: 这里解释一下为什么隐藏武器时要把子对象 Active 给设置成
                // false 而不是直接设置自己，这是因为如果把自己的 Active 给设置成
                // false 的话，那这个脚本也会失效！！！上面那个协同程序也就无法继续
                // 执行了！！！
                child.gameObject.SetActive(shouldShow);
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButton(0))
            {
                Pickup(playerController.gameObject);
            }

            //Note: 这里返回true是因为要让这个物品永远可以捡起来，如果返回
            //false的话，鼠标就会变成forbid样式的，因为后面没有可以处理射线
            //的Component了
            return true;
        }
    }
}
