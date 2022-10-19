using System;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // CONFIG DATA
        
        [Tooltip("光标映射结构体数组")]
        [SerializeField] private CursorMapping[] cursorMappings = null;
        [Tooltip("navmesh最大误差距离")]
        [SerializeField] private float maxNavMeshProjectionDistance = 1.0f;
        [Space(10)]
        [Tooltip("射线半径")]
        [SerializeField] private float raycastRadius = 1f;

        /// <summary>
        /// 光标映射结构体
        /// </summary>
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;     // 光标类型
            public Texture2D texture;   // 纹理图片
            public Vector2 hotspot;     // 热点区域
        }

        // CACHE & STATE
        
        private Camera _mainCamera;             // 主摄像机
        private Mover _mover;                   // 角色移动脚本
        private Fighter _fighter;               // 角色攻击脚本
        private Health _health;                 // 角色生命脚本
        private bool isDraggingUI = false;      // 是否在拖动UI

        // LIFECYCLE METHODS
        
        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            
            _mainCamera = Camera.main;
        }

        #region 废弃代码

        // void Start()
        // {
        //     _mainCamera = Camera.main;
        // }

        #endregion
        
        void Update()
        {
            if (InteractWithUI()) return;
            
            if (_health.IsDead())
            {
                SetCursor(CursorType.Forbid);
                return;
            }

            if (InteractWithComponent()) return;

            #region 废弃代码

            /**********************************************************
             * 这里InteractWithCombat()放在前面先判断是为了点击的敌人的时候
             * 不会自动跑到敌人身上去
             **********************************************************/
            // if (InteractWithCombat()) return;

            #endregion

            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.Forbid);

            #region 废弃代码

            // InteractMovement();
            // Debug.Log("无事可做");

            #endregion
        }
        
        // PRIVATE

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDraggingUI)
            {
                // 如果目前正在与UI做交互则返回true
                // 阻断鼠标与其他物体的交互逻辑
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            Ray ray = GetMouseRay();

            // RaycastHit[] hits = new RaycastHit[10];
            // Physics.SphereCastNonAlloc(ray, raycastRadius, hits);

            RaycastHit[] hits = Physics.SphereCastAll(ray, raycastRadius);

            //对hits数组进行排序，因为unity默认不是根据射线碰撞的
            //深度来排好序的，默认是随机顺序
            RaycastAllSorted(hits);
            
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] components = hit.transform.GetComponents<IRaycastable>();
                
                foreach (IRaycastable raycastable in components)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        //返回true，说明射线已经被处理了，可以直接返回了，不用
                        //再往下循环了
                        
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        /***********************************************************
         * InteractMovement()和InteractWithCombat()的返回值改为bool
         * 是为了鼠标指针UI上的交互，如果指针悬停在敌人身上就会变成攻击图标
         * 停在了地图其他地方就变成默认或者是移动的图标，而且如果这两个都返
         * 回的false的话就说明鼠标指针指向了地图外面，则不作处理
         ***********************************************************/
        private bool InteractWithMovement()
        {
            // Ray lastRay = GetMouseRay();
            // bool hasHit = Physics.Raycast(lastRay, out RaycastHit hit);
            
            bool hasHit = RaycastNavMesh(out Vector3 target);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    // _mover.StartMoveAction(hit.point, 1f);
                    _mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Move);
                return true;
            }

            SetCursor(CursorType.Forbid);
            return false;
        }

        //在NavMesh上找到与Raycast碰撞到的位置最近的点
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            // if (!GetComponent<NavMeshAgent>().enabled) return false;
            
            Ray lastRay = GetMouseRay();
            bool hasHit = Physics.Raycast(lastRay, out RaycastHit hit);
            if (!hasHit) return false;

            // NavMeshHit navMeshHit = new NavMeshHit();
            bool hasFound = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance,
                NavMesh.AllAreas);
            if (!hasFound) return false;
            
            target = navMeshHit.position;
            
            //下面开始剔除一些NavMesh中不合理的路径
            //比如说房顶的一小块网格，比如超远距离的路径
            NavMeshPath path = new NavMeshPath();
            //如果没要找到路径
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;

            //如果路径不能到达目的地
            //这里判断的是路径是否完整，如果只是部分路径（残缺）的话会返回false
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            
            //TODO:如果路径长度大于设定值
            
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        /// <summary>
        /// 对所有与鼠标射线碰撞的对象根据距离进行排序
        /// </summary>
        /// <param name="hits">hits数组</param>
        /// <returns>返回排序后的hits数组</returns>
        private RaycastHit[] RaycastAllSorted(RaycastHit[] hits)
        {
            float[] distance = new float[hits.Length];
            for (var i = 0; i < hits.Length; i++)
            {
                distance[i] = hits[i].distance;
            }
            
            //默认是从小到大排序
            Array.Sort(distance,hits);

            return hits;
        }

        #region 废弃代码

        // private bool InteractWithCombat()
        // {
        //     Ray ray = GetMouseRay();
        //     RaycastHit[] hits  = Physics.RaycastAll(ray);
        //     foreach (var hit in hits)
        //     {
        //         CombatTarget target = hit.transform.GetComponent<CombatTarget>();
        //         if (target == null) continue;
        //         if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;
        //
        //         if (Input.GetMouseButton(0))
        //         {
        //             _fighter.Attack(target.gameObject);
        //         }
        //         SetCursor(CursorType.Attack);
        //         return true;
        //     }
        //
        //     return false;
        // }

        #endregion

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            for (int i = 0; i < cursorMappings.Length; i++)
            {
                if (cursorMappings[i].type == type)
                {
                    return cursorMappings[i];
                }
            }
            return cursorMappings[0];
        }
        
        private Ray GetMouseRay()
        {
            return _mainCamera.ScreenPointToRay(Input.mousePosition);
        }
    }
}