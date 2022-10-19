using RPG.AttTypeDefine;
using RPG.Control;
using UnityEngine;

namespace RPG.FiniteStateMechine
{
    public class FSM_Chase : FSMState
    {
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        public FSM_Chase(AIController ai) : base(eEnemyState.eChase, ai) {}
        
        public override void OnUpdate()
        {
            base.OnUpdate();

            _timeSinceLastSawPlayer = 0;
            
        }
    }
}
