using GameDevTV.Inventories;
using RPG.Core;
using UnityEngine;
using RPG.Movement;
using GameDevTV.Saving;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour,IAction,ISaveable
    {
        // CONFIG DATA
        
        [Tooltip("攻击速度")]
        [SerializeField] private float attackSpeed = 1f;
        [Tooltip("左手Transform（用于装备武器以及销毁武器，下同）")]
        [SerializeField] private Transform leftHandSpawnTransform = null;
        [Tooltip("右手Transform")]
        [SerializeField] private Transform rightHandSpawnTransform = null;
        [Tooltip("默认武器")]
        [SerializeField] private WeaponConfig defaultWeaponConfig = null;
        [Tooltip("敌人的追击速度因子")] [Range(0, 1)] [SerializeField]
        private float enemyChaseVelocityFactory = 0.6f;

        #region 废弃代码

        /// <summary>
        /// 默认武器名，根据名字来动态加载武器
        /// </summary>
        // [SerializeField] private string defaultWeaponName = "Unarmed";

        #endregion
        
        // STATE

        #region 废弃代码

        // private float _timeSinceLastAttack = 0f;
        // private NavMeshAgent _agent;
        // private Transform _target;

        #endregion
        private float _timeSinceLastAttack = Mathf.Infinity;    //可以让一开始就大于上一次的攻击时间间隔
        private Health _target; //改用Health来指定target

        // CACHE REFERENCES

        private Mover _mover;                           // 角色移动脚本
        private ActionScheduler _scheduler;             // 行为调度器
        private Animator _animator;                     // 角色动画器
        private static readonly int triggerAttack = Animator.StringToHash("attack");
        private WeaponConfig _currentWeaponConfig;      // 当前武器配置
        private Weapon _currentWeapon;
        private Equipment _equipment;                   // 角色装备
        public Health Target
        {
            get => _target;
        }
        
        // LIFECYCLE METHODS
        
        private void Awake()
        {
            // 获取脚本
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _equipment = GetComponent<Equipment>();
            
            // 游戏启动给角色装备武器
            if (_currentWeaponConfig == null)
            {
                _currentWeapon = EquipWeapon(defaultWeaponConfig);
            }
            
            // 订阅装备面板更新的消息
            if (_equipment)
            {
                _equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        #region 废弃代码

        // private void Start()
        // {
        //     // _agent = GetComponent<NavMeshAgent>();
        //
        //     // Weapon weapon = Resources.Load<Weapon>(defaultWeaponName);
        //     if (currentWeapon == null)
        //     {
        //         EquipWeapon(defaultWeapon);
        //     }
        // }

        #endregion

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            // 如果_target为null，说明没有点击敌人，这时必须return掉，
            // 不然会一直执行后面else里的 Stop()方法，角色会走不动路
            if (_target == null) return;    // 没有敌人时直接返回，不执行下面的攻击逻辑
            if (_target.IsDead()) return;   // 玩家死亡时直接返回，不执行下面的攻击逻辑
            
            /****************************************************************
             * 这里有个小技巧！把isInRange变量的判断改成GetIsInRange()函数内联变量
             * 的形式可以避免产生_target的空引用异常，因为前面第一个判断条件已经判断了
             * 它是否为空，如果为空的话也不会再判断第二个判断条件了！！
             ****************************************************************/
            if (_target != null && !GetIsInAttackRange())
            {   
                // 如果攻击目标不为空而且不在攻击范围内时，会移动到目标位置
                _mover.MoveTo(_target.transform.position, enemyChaseVelocityFactory);
            }
            else
            {
                // 如果目标消失或者已经移动到攻击范围内时则停止移动开始攻击逻辑
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        //这个函数的功能变了，所以函数名从SpawnWeapon更改为EquipWeapon
        public Weapon EquipWeapon(WeaponConfig weaponConfig)
        {
            // if (_weapon == null) return;
            _currentWeaponConfig = weaponConfig;
            // Instantiate(weaponPrefab, weaponSpawnTransform);
            Animator animator = GetComponent<Animator>();
            // animator.runtimeAnimatorController = weaponOverrideController;
            return _currentWeaponConfig.Spawn(leftHandSpawnTransform, rightHandSpawnTransform, animator);
        }

        /// <summary>
        /// 装备面板的武器槽更新时，该方法会接收到广播并执行
        /// </summary>
        private void UpdateWeapon()
        {
            // 获取装备栏上的武器
            WeaponConfig weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            
            // 判断是否为空
            if (weapon == null)
            {
                EquipWeapon(defaultWeaponConfig);   // 装备默认武器
            }
            else
            {
                EquipWeapon(weapon);    // 装备武器槽上的武器
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (_timeSinceLastAttack > attackSpeed)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger(triggerAttack);
        }

        private bool GetIsInAttackRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeaponConfig.WeaponRange;
        }

        /**
         * 为了泛用性，把参数类型从CombatTarget改为GameObject，因为
         * Player身上是不会挂CombatTarget组件的所以为了让敌人也能攻击
         * 玩家而做此修改
         */
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false; // 不要忘记如果combatTarget为空的情况下要直接返回false
            Health healthToTest = combatTarget.GetComponent<Health>();
            return healthToTest != null && !healthToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            // 开始攻击行为
            GetComponent<ActionScheduler>().StartAction(this);
            // 设置攻击目标
            _target = combatTarget.GetComponent<Health>();
            // 看向目标
            transform.LookAt(_target.transform);
        }

        #region 废弃代码

        // 这里可以理解为把对伤害的增益都添加到了一个列表里面
        // yield return 就相当于添加的操作，返回一个IEnumerable<float>
        // 它里面就包含了对这个列表的一个迭代器
        // IEnumerable<float> IModifierProvider.GetAdditiveModifiers(StatEnum stat)
        // {
        //     if (stat == StatEnum.Attack)
        //     {
        //         // 添加武器对伤害的增益
        //         yield return _currentWeaponConfig.WeaponDamage;
        //     }
        // }
        
        // 获取所有的百分比加成
        // IEnumerable<float> IModifierProvider.GetPercentageModifiers(StatEnum stat)
        // {
        //     if (stat == StatEnum.Attack)
        //     {
        //         // 添加百分比攻击力加成
        //         yield return _currentWeaponConfig.AttackPercentageBonus;
        //     }
        // }

        #endregion

        public void Cancel()
        {
            StopTriggerAttack();
            _target = null;
            
            //如果中途取消攻击，在没到攻击范围内时也要取消移动
            // _mover.Cancel();
            GetComponent<ActionScheduler>().CancelCurrentAction();;
        }

        private void StopTriggerAttack()
        {
            _animator.ResetTrigger(triggerAttack);
            _animator.SetTrigger("stopAttack");
        }

        #region AnimationEvents动画事件

        void Shoot()
        {
            Hit();
        }
        
        void Hit()
        {
            // _target.GetComponent<Health>().DoDamage(weaponDamage);
            if (_target == null) return;    //避免空指针

            float damage = GetComponent<BasicStats>().GetStat(StatEnum.Attack);

            Debug.Log(gameObject.name + "take damage: " + damage);

            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(gameObject, leftHandSpawnTransform, rightHandSpawnTransform, _target, damage);
            }
            else
            {
                // _target.DoDamage(gameObject, currentWeapon.WeaponDamage);
                _target.DoDamage(gameObject, damage);
            }
        }

        #endregion

        #region 保存和恢复逻辑

        public object CaptureState()
        {
            //切换场景时保存武器
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            //切换场景后恢复武器
            string weaponName = (string) state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }

        #endregion
    }
}