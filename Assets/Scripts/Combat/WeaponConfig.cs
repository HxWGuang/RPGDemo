using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// 存储不同武器的各种属性
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon",menuName = "RPG/Weapons/Make New Weapon",order = 0)]
    public class WeaponConfig : EquipableItem,IModifierProvider
    {
        // CONFIG DATA
        
        [Tooltip("武器预制体（拿在手上的EquippedWeapon）")]
        [SerializeField] private Weapon weaponPrefab = null;
        [Tooltip("该武器的重载动画控制器")]
        [SerializeField] private AnimatorOverrideController weaponOverrideController = null;
        [Space(10)]
        [Tooltip("武器攻击范围")]
        [SerializeField] private float weaponRange = 2f;
        [Tooltip("武器伤害")]
        [SerializeField] private float weaponDamage = 5f;
        [Tooltip("百分比攻击力加成")]
        [SerializeField] private float attackPercentageBonus = 0f;
        [Tooltip("是否是装备在右手上")]
        [SerializeField] private bool isRightHand = true;
        [Space(10)] 
        [Tooltip("远程武器的投掷物脚本")]
        [SerializeField] private Projectile projectile = null;

        private const string weaponName = "Weapon";

        public float WeaponRange => weaponRange;
        public float WeaponDamage => weaponDamage;
        public float AttackPercentageBonus => attackPercentageBonus;

        public bool HasProjectile()
        {
            return projectile != null ? true : false;
        }
        
        /// <summary>
        /// 拾取武器时，根据武器类型在左手或右手上生成武器，以及替换为
        /// 武器对应的AnimatorOverrideController
        /// </summary>
        /// <param name="leftHand"></param>
        /// <param name="rightHand"></param>
        /// <param name="animator"></param>
        public Weapon Spawn(Transform leftHand, Transform rightHand, Animator animator)
        {
            // 1
            // 销毁原有武器
            DestroyOldWeapon(leftHand, rightHand);

            // 2
            // 生成新的武器
            Weapon weapon = null;
            if (weaponPrefab != null)   //有些武器没有装备在手上时的预制体，比如蹦蹦炸弹
            {
                weapon = Instantiate(weaponPrefab, GetTransform(leftHand, rightHand));
                weapon.gameObject.name = weaponName;  //起名是为了调用DestroyOldWeapon销毁武器时不混淆
            }
            
            // 3
            //判断当前的AnimatorController是原生的还是已经被覆盖过的AnimatorOverrideController
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverrideController != null)  //当前武器有weaponOverrideController
            {
                animator.runtimeAnimatorController = weaponOverrideController;
            } else if (overrideController != null)  //如果某个武器没有设置AnimatorOverrideController设置回默认的AnimatorController
            {                                       //而不是继续使用上一个覆盖掉AnimatorController的AnimatorOverrideController
                
                //overrideController != null 说明当前的animator controller是AnimatorOverrideController
                //也就是原生controller已经被替换过了，通过AnimatorOverrideController的runtimeAnimatorController
                //属性可以反向找到被替换掉的原生controller
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        /// <summary>
        /// 发射远程武器飞行物
        /// </summary>
        /// <param name="instigator">发动者</param>
        /// <param name="leftHand">左手transform</param>
        /// <param name="rightHand">右手transform</param>
        /// <param name="target">攻击目标</param>
        /// <param name="damage">造成的伤害</param>
        public void LaunchProjectile(GameObject instigator, Transform leftHand, Transform rightHand, Health target, float damage)
        {
            Projectile projectileInstance =
                Instantiate(projectile, GetTransform(leftHand, rightHand).position, Quaternion.identity);
            projectileInstance.SetTarget(instigator, target, damage);
        }

        /// <summary>
        /// 销毁装备在手上的武器
        /// </summary>
        /// <param name="leftHand">左手Transform</param>
        /// <param name="rightHand">右手Transform<</param>
        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            // 找出当前武器在哪只手上
            Transform oldWeapon = leftHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = rightHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            
            //因为一些顺序问题或帧问题或调用问题，在销毁前需要把名字改一下
            //从本质上讲，这将确保我们不会出现这样的问题，即我们拿起了新的
            //武器，而我们却混淆了这是我们一秒钟前刚刚拿起的新武器，还是我
            //们手中已经有的旧武器？
            //所以在销毁旧武器时。我们要找到那个旧武器，把它重命名为销毁，
            //然后这将使我们能够销毁旧武器，而不会在名字上产生混淆。命名
            //新的被命名的武器等等，所以这只是我们通过游戏测试发现的一个问
            //题，如果我们不在摧毁它之前正确地重命名它，我们就会有一点问题。
            oldWeapon.name = "DESTROYING";  //把要销毁的对象改名以防止出现一些bug

            //默认的Destroy()方法是延迟销毁的，会在一帧的最后执行
            Destroy(oldWeapon.gameObject);
        }

        /// <summary>
        /// 根据当前武器是左手武器还是右手武器来返回对应的Transform
        /// </summary>
        /// <param name="leftHand">左手的transform</param>
        /// <param name="rightHand">右手的transform</param>
        /// <returns>对应的手部transform</returns>
        private Transform GetTransform(Transform leftHand, Transform rightHand)
        {
            var transform = isRightHand ? rightHand : leftHand;
            return transform;
        }

        #region 迭代武器对角色相关属性的加成

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatEnum stat)
        {
            if (stat == StatEnum.Attack)
            {
                // 添加武器对伤害的增益
                yield return WeaponDamage;
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatEnum stat)
        {
            if (stat == StatEnum.Attack)
            {
                // 添加百分比攻击力加成
                yield return attackPercentageBonus;
            }
        }

        #endregion
    }
}