using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        /// <summary>
        /// 获取所有指定属性的固定数值增益
        /// </summary>
        /// <param name="stat">指定的状态</param>
        /// <returns>数值总和</returns>
        IEnumerable<float> GetAdditiveModifiers(StatEnum stat);

        IEnumerable<float> GetPercentageModifiers(StatEnum stat);
    }
}