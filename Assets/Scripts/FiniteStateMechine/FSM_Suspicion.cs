using RPG.AttTypeDefine;
using RPG.Control;
using RPG.FiniteStateMechine;

public class FSM_Suspicion : FSMState
{
    public FSM_Suspicion(AIController ai) : base(eEnemyState.eSuspicion, ai) {}

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        
    }
}
