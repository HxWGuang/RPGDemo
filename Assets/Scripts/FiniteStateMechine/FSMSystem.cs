using System.Collections.Generic;
using RPG.AttTypeDefine;
using UnityEngine;

namespace RPG.FiniteStateMechine
{
    public class FSMSystem
    {
        public Dictionary<eEnemyState, FSMState> DicState;
        public FSMState CurState;

        public void OnStart()
        {
            DicState = new Dictionary<eEnemyState, FSMState>();
        }

        public void AddState(FSMState state)
        {
            if (null != state && !DicState.ContainsValue(state))
            {
                if (DicState.Count == 0)
                {
                    CurState = state;
                    CurState.OnStart();
                }

                DicState.Add(state.StateId, state);
            }
        }

        public void RemoveState(FSMState state)
        {
            if (null != state && DicState.ContainsValue(state))
            {
                DicState.Remove(state.StateId);
            }
        }

        public void SetTransition(eEnemyState id)
        {
            // 新老状态切换
            // 新：id
            // 老：CurState

            var temNew = DicState[id];

            if (null == temNew)
            {
                Debug.LogError($"转换失败，无此状态：({id})");
                return;
            }
            else
            {
                CurState.OnEnd();
                CurState = temNew;
                CurState.OnStart();
            }
        }
    }
}
