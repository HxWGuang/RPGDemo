using RPG.AttTypeDefine;
using RPG.Control;
using UnityEngine;

namespace RPG.FiniteStateMechine
{
    public class FSMBehaviour : MonoBehaviour
    {
        private FSMSystem SysInst;
        [HideInInspector]
        public AIController Owner;

        private void Update()
        {
            if (null != SysInst && null != SysInst.CurState)
            {
                // 驱动当前状态
                SysInst.CurState.OnUpdate();
            }
        }

        public void OnStart(AIController en)
        {
            Owner = en;
            SysInst = new FSMSystem();
            SysInst.OnStart();

            InitFSM();
        }

        void InitFSM()
        {
            var idle = new FSM_Idle(Owner);
            var chase = new FSM_Chase(Owner);
            
            SysInst.AddState(idle);
            SysInst.AddState(chase);
        }

        public void SetTransition(eEnemyState id)
        {
            SysInst.SetTransition(id);
        }
    }
}
