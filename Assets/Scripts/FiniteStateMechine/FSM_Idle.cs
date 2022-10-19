using RPG.AttTypeDefine;
using RPG.Control;
using UnityEngine;

namespace RPG.FiniteStateMechine
{
    public class FSM_Idle : FSMState
    {
        float chaseDis = 3f;
        public FSM_Idle(AIController ai) : base(eEnemyState.eIdle, ai){}
    
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (IsAggrevated())
            {
                Owner.FSMInst.SetTransition(eEnemyState.eChase);
            }
        }
    
        private bool IsAggrevated()
        {
            var distanceToPlayer = Vector3.Distance(Owner.transform.position, PlayInst.transform.position);
            // return distanceToPlayer < Owner.FSMInst.chaseDis || _timeSinceAggrevated < aggroCoolDownTime;
            return distanceToPlayer < chaseDis;
        }
    }
}
