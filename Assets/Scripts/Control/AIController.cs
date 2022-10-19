using RPG.Combat;
using RPG.Core;
using RPG.FiniteStateMechine;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // CONFIG DATA
        
        [Tooltip("追击距离")]
        [SerializeField] private float chaseDistance = 5f;
        [Tooltip("警戒时间(逃脱嫌疑时间)")]
        [SerializeField] private float suspicionTime = 3f;
        [Tooltip("被激怒后的冷却时间，超过时间后就不再是被激怒状态")]
        [SerializeField] private float aggroCoolDownTime = 1.5f;
        [Tooltip("被攻击后激怒指定半径范围内的所有敌人")]
        [SerializeField] private float alertRadius = 5f;
        [Space(10)]
        [Tooltip("距离误差")]
        [SerializeField] private float distanceTolerance = 0.5f;
        [Tooltip("巡逻路径")]
        [SerializeField] private PatrolPath patrolPath;
        [Tooltip("到达路径点后的停顿时间")]
        [SerializeField] private float dwellTime = 2f;
        [Range(0,1)]
        [Tooltip("巡逻时速度变化因子")]
        [SerializeField] private float patrolSpeedFactory = 0.2f;
        
        // STATE

        public GameObject PlayInst;
        private Transform _playerTransform;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        
        private Vector3 _guardPosition;
        private Vector3 _nextPosition;
        private int _currentWayPointIndex = 0;

        /// <summary>
        /// 在路径点上的逗留时间
        /// </summary>
        private float _timeSinceDwellLastWayPoint = Mathf.Infinity;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        private bool _haveAggrevatedNearbyEnemies = false;

        private void Awake()
        {
            PlayInst = GameObject.FindWithTag("Player");
            _playerTransform = PlayInst.transform;
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            
            _guardPosition = transform.position;
        }

        private void Start()
        {
            fsmInst = gameObject.AddComponent<FSMBehaviour>();
            fsmInst.OnStart(this);
        }

        #region 废弃代码

        // private void Start()
        // {
        //     _guardPosition = transform.position;
        // }

        #endregion

        private void Update()
        {
            if (_health.IsDead()) return;
            
            if (IsAggrevated() && _fighter.CanAttack(_playerTransform.gameObject))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceDwellLastWayPoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        #region FSMBehaviour

        private FSMBehaviour fsmInst;
        public FSMBehaviour FSMInst => fsmInst;

        #endregion

        private void PatrolBehaviour()
        {
            if (patrolPath == null)
            {
                _nextPosition = _guardPosition;
            }
            else
            {
                if (AtWaypoint())   //是否在巡逻路径点上
                {
                    _timeSinceDwellLastWayPoint = 0;
                    CircleWayPoint();
                }

                _nextPosition = GetCurrentWayPoint();
            }

            if (_timeSinceDwellLastWayPoint > dwellTime)
            {
                _mover.StartMoveAction(_nextPosition, patrolSpeedFactory);
            }
        }

        private void CircleWayPoint()
        {
            _currentWayPointIndex = patrolPath.GetNextIndex(_currentWayPointIndex);
        }

        private bool AtWaypoint()
        {
            float distance = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distance < distanceTolerance;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(_currentWayPointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            _haveAggrevatedNearbyEnemies = false;
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_playerTransform.gameObject);

            if (!_haveAggrevatedNearbyEnemies)
            {
                AggrevateNearbyEnemies();
            }
        }

        //激怒附近敌人
        private void AggrevateNearbyEnemies()
        {
            _haveAggrevatedNearbyEnemies = true;
            
            RaycastHit[] results = new RaycastHit[10];  //最多检测10个
            Physics.SphereCastNonAlloc(transform.position, alertRadius, Vector3.up,results, 0);
            foreach (RaycastHit hit in results)
            {
                // hit也有可能为null
                if (hit.collider ==  null) continue;
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;
                
                ai.Aggrevated();
            }
        }
        
        // 敌人被攻击激怒后会由UnityEvent调用
        // 该方法要被UnityEvent调用必须要是public！！！
        public void Aggrevated()
        {
            _timeSinceAggrevated = 0;
        }

        //这里把方法名从IsPlayerInChaseRange改为IsAggrevated，因为敌人发动
        //攻击不仅取决于是否进入了他的警觉范围也取决于是否被玩家攻击了
        private bool IsAggrevated()
        {
            var distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            return distanceToPlayer < chaseDistance || _timeSinceAggrevated < aggroCoolDownTime;
        }

        // 绘制追击范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        #region 废弃代码

        // private float DistanceToPlayer()
        // {
        //     // GameObject player = GameObject.FindWithTag("Player");
        //     // return Vector3.Distance(transform.position, player.transform.position);
        //     return Vector3.Distance(transform.position, _chaseTarget.position);
        // }

        #endregion
    }
}