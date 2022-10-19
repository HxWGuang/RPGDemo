using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        // CONFIG DATA
        [Tooltip("投射物飞行速度")]
        [SerializeField] private float speed = 1f;
        [Tooltip("是否是指向性攻击")]
        [SerializeField] private bool isHoming = true;
        [Tooltip("投射物发生碰撞时的效果")]
        [SerializeField] private GameObject hitEffect = null;
        [Tooltip("投射物最大生命时间")]
        [SerializeField] private float maxLiftTime = 7f;
        [Tooltip("发生碰撞时要立即销毁的对象")]
        [SerializeField] private GameObject[] destroyOnHit = null;
        [Tooltip("发生碰撞后投射物的销毁时间")]
        [SerializeField] private float lifeAfterHit = 2f;

        // STATE DATA
        private GameObject instigator = null;   // 攻击发起者
        private Health target = null;           // 攻击目标
        private float damage = 0;               // 造成的伤害

        private void Start()
        {
            // 刚生成对象时就指向目标
            transform.LookAt(GetAimTransform());
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming && !target.IsDead())   //是指向性且敌人没死的情况下就一直追踪
            {
                transform.LookAt(GetAimTransform());
            }
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }

        /// <summary>
        /// 设置攻击目标
        /// </summary>
        /// <param name="instigator">攻击发起者</param>
        /// <param name="target">攻击目标</param>
        /// <param name="damage">伤害值</param>
        public void SetTarget(GameObject instigator, Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            
            // 经过最大生命时间后进行销毁
            Destroy(gameObject, maxLiftTime);
        }

        private Vector3 GetAimTransform()
        {
            //借助碰撞体来计算对象的中心位置
            CapsuleCollider capsuleCollider = target.GetComponent<CapsuleCollider>();
            if (capsuleCollider == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * capsuleCollider.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;

            // 如果该投射物有碰撞特效则生成特效
            if (hitEffect != null)
            {
                // 生成碰撞特效，投射物的发射特效在投射物预制体上
                Instantiate(hitEffect, GetAimTransform(), transform.rotation);
            }

            target.DoDamage(instigator, damage);

            //撞击后让投射物速度归零
            speed = 0;
            
            //销毁需要立即销毁的对象
            foreach (var o in destroyOnHit)
            {
                Destroy(o);
            }

            //延迟销毁本体
            Destroy(gameObject, lifeAfterHit);
        }
    }
}