using System;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour,IAction, ISaveable
    {
        // CONFIG DATA
        [Tooltip("移动的最大速度")]
        [SerializeField] private float maxSpeed = 6f;
        
        // STATE DATA
        private NavMeshAgent _agent;
        private Animator animator;
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private Health _health;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            // TryGetComponent<Animator>(out animator);
            animator = GetComponent<Animator>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            _agent.enabled = !_health.IsDead();
            UpdateAnim();
        }

        public void StartMoveAction(Vector3 destination, float speedFactory)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFactory);
            
            // GetComponent<Fighter>().Cancel();   // 这里产生了依赖循环，Movement依赖了Combat
        }
    
        //由Navmesh驱动
        public void MoveTo(Vector3 destination, float speedFactory)
        {
            _agent.destination = destination;
            _agent.speed = maxSpeed * Mathf.Clamp01(speedFactory);  //锁定速度因子在0-1之间
            _agent.isStopped = false;
        }
        
        public void Cancel()
        {
            _agent.isStopped = true;
        }

        private void UpdateAnim()
        {
            // Vector3 speed = _agent.velocity;
            // Note: 这里需要把agent的速度转到本地坐标系，因为agent的速度是在世界
            // 坐标系中的，向量是带方向的，下面又要用到z方向上的速度分量，所以要转到
            // 本地坐标系，如果不转的话那么只要方向在世界的前方时才会有速度，所以人往
            // 世界的后方走的话z就是0，那么动画就不会动
            Vector3 speed = transform.InverseTransformDirection(_agent.velocity);
            animator.SetFloat(MoveSpeed,speed.z);
        }

        #region 存储与加载位置

        object ISaveable.CaptureState()
        {
            //保存位置
            return new SerializableVector3(transform.position);
        }

        void ISaveable.RestoreState(object state)
        {
            //获取位置
            SerializableVector3 position = (SerializableVector3) state;
            GetComponent<NavMeshAgent>().Warp(position.ToVector());
        }

        #endregion
    }
}