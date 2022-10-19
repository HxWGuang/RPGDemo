using RPG.AttTypeDefine;
using RPG.Control;
using UnityEngine;

namespace RPG.FiniteStateMechine
{
    public abstract class FSMState
    {
        public eEnemyState StateId => stateId;
        private eEnemyState stateId;
        protected AIController Owner;
        protected GameObject PlayInst;
        public FSMState(eEnemyState id, AIController ai)
        {
            stateId = id;
            Owner = ai;
            PlayInst = Owner.PlayInst;
        }
        
        public virtual void OnStart(){}
        public virtual void OnUpdate(){}
        public virtual void OnEnd(){}
    }
}
