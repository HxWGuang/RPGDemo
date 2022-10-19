using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction currentAction;      // 当前行为

        public void StartAction(IAction action)
        {
            // 判断开始的行为是否与当前的行为相同，相同则直接返回
            if (currentAction == action) return;  //不取消自己
            
            // 设置当前行为为传入的新行为
            if (currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}