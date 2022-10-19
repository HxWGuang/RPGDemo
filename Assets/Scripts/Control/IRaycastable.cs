namespace RPG.Control
{
    public interface IRaycastable
    {
        /// <summary>
        /// 返回当前Component接管射线后，需要返回哪种鼠标指针类型
        /// </summary>
        /// <returns></returns>
        CursorType GetCursorType();
        
        /// <summary>
        /// 处理(接管)射线
        /// </summary>
        /// <returns>
        /// 返回true说明当前的Component已经接管了射线，那么就
        /// 不会再往下循环寻找可以接管的Component；返回false说明当前
        /// Component接管不了射线，则往后继续寻找
        /// </returns>
        bool HandleRaycast(PlayerController playerController);
    }
}