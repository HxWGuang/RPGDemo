using System;
using GameDevTV.Inventories;
using RPG.Core;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Stats
{
    /**
 * Health从Combat移到了Core，因为Health作为底层依赖被许多其他类所依赖
 * Health类要更为稳定，所以移动到Core命名空间中
 */
    public class Health : MonoBehaviour, ISaveable
    {
        [Serializable]
        public class GetDamageEvent: UnityEvent<float,Vector3>{}
        [Serializable]
        public class HealthChangeEvent : UnityEvent<float>{}

        /// <summary>
        /// 升级时生命值若低于设定的百分比则把生命值恢复到指定百分比
        /// </summary>
        [SerializeField] private float regenerateHpPercentage = 70f;

        //采用Unity的事件来传递伤害值
        [HideInInspector] public GetDamageEvent getDamageEvent;
        public HealthChangeEvent healthChangeEvent;
        public UnityEvent OnBeenAttacked;
        public UnityEvent OnDie;

        #region 废弃代码

        // 这里使用的是 C# 的 委托
        // public event Action<float, Vector3> TakeDamageEvent; 

        // /// <summary>
        // /// 生命值
        // /// </summary>
        // [SerializeField] private float health = 100;

        #endregion

        // Note:这里把health设置成-1是因为敌人死亡后重新加载存档
        // 会先执行RestoreState()恢复存档数据把敌人生命值赋成0，
        // 然后又会执行Start()里的赋值代码把敌人生命值重新赋成初始值
        // 导致存档的数据被覆盖掉了
        private float health = -1f;
        // private LazyValue<float> health;

        public float HealthPoint
        {
            get => health;
        }
        public float MaxHealthPoint
        {
            get => maxHealth;
        }

        private float maxHealth;
        private bool _isDied = false;
        private BasicStats _basicStats = null;
        
        // 获取装备
        private Equipment _equipment;
        
        
        private void Awake()
        {
            // Debug.Log("Health Awake执行");
            
            _basicStats = GetComponent<BasicStats>();
            // _damageUI = FindObjectOfType<DamageUI>();
        }

        private void OnEnable()
        {
            if (gameObject.CompareTag("Player"))    //玩家才需要执行下面的操作
            {
                _basicStats.OnLevelUp += RegenerateHp;
                
                // 订阅装备更新事件
                _equipment = GetComponent<Equipment>();
                if (_equipment)
                {
                    _equipment.equipmentUpdated += UpdateHealth;
                }
            }
        }

        private void Start()
        {
            //这里经常会出Bug就是因为我们在Health的Awake里面调用_basicStats.GetStat
            //而这个方法需要用到currentLevel，但这个参数是在BasicStat的Awake里进行初始化
            //所以这里会发生数据读写冲突，基本的解决方法应该是BasicStat的Awake要比Health
            //的Awake先执行才可以，暂时的解决方法：把这里获取health的代码移到Start里面
            // 初始化默认生命值（注意：由startLevel指定的等级）
            if (health < 0) //health < 0 说明是第一次加载游戏
            {
                health = _basicStats.GetStat(StatEnum.Health);
            }
            maxHealth = _basicStats.GetStat(StatEnum.Health);
        }

        private void OnDisable()
        {
            if (gameObject.CompareTag("Player"))    //玩家才需要执行下面的操作
            {
                _basicStats.OnLevelUp -= RegenerateHp;
            }
        }


        // private void Start()
        // {
        //     Debug.Log("Health Start执行");
        // }

        // private void Start()
        // {
        //     // 初始化默认生命值（注意：由startLevel指定的等级）
        //     if (health < 0) //health < 0 说明不是从存档中加载的数据
        //     {
        //         health = _basicStats.GetStat(StatEnum.Health);
        //     }
        //     maxHealth = _basicStats.GetStat(StatEnum.Health);
        // }

        public float GetHealthPercent()
        {
            return GetFraction() * 100;
        }

        private float GetFraction()
        {
            return (health / maxHealth);
        }

        /// <summary>
        /// 攻击造成伤害
        /// </summary>
        /// <param name="instigator">攻击发起者</param>
        /// <param name="damage">伤害数值</param>
        public void DoDamage(GameObject instigator ,float damage)
        {
            if (_isDied) return;

            health = Mathf.Max(health - damage, 0);
            
            // _damageUI.NewDamage(damage, transform.position);
            // 采用事件的方式来代替(Tips:现在不能用事件，原因暂不明)
            getDamageEvent.Invoke(damage, transform.position); //显示伤害数字
            healthChangeEvent.Invoke(GetFraction());    //血条扣血缓冲
            // Debug.Log(takeDamageEvent.GetPersistentEventCount());
            
            if (health <= 0)
            {
                Die();

                if (!gameObject.CompareTag("Player")) //敌人杀死玩家不会获得经验
                {
                    //玩家在敌人死亡时获得经验值
                    AwardExp(instigator);
                }
            }
            else
            {
                OnBeenAttacked.Invoke();    //被攻击音效
            }
        }

        private void Die()
        {
            OnDie.Invoke();
            _isDied = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExp(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            experience.GainExp(GetComponent<BasicStats>().GetStat(StatEnum.RewardExp));
        }

        /// <summary>
        /// 升级时被调用
        /// </summary>
        private void RegenerateHp()
        {
            maxHealth = GetCurrentLevelMaxHealth();
            float regenHp = maxHealth * (regenerateHpPercentage / 100);
            // Debug.Log("修改health前：health = " + health);
            health = Mathf.Max(health, regenHp);

            healthChangeEvent.Invoke(GetFraction());
            // Debug.Log("maxHP = " + maxHealth + "  " + "hp = " + health + "  " + "regenHp = " + regenHp);
        }

        public void Heal(float healValue)
        {
            health = Mathf.Min(health + healValue, GetCurrentLevelMaxHealth());
            healthChangeEvent.Invoke(GetFraction());
        }

        private float GetCurrentLevelMaxHealth()
        {
            return _basicStats.GetStat(StatEnum.Health);
        }

        private void FullHealth()
        {
            health = GetCurrentLevelMaxHealth();
            maxHealth = health;
        }

        private void UpdateHealth()
        {
            maxHealth = GetCurrentLevelMaxHealth();
        }

        public bool IsDead()
        {
            return _isDied;
        }

        #region 保存操作

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            // Debug.Log("Health Restore执行");
            
            health = (float) state;
            
            //加载完后需要再检查一遍对象是否已经死亡
            if (health <= 0)
            {
                Die();
            }
        }

        #endregion
    }
}