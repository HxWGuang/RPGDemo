using System.Collections;
using RPG.Stats;
using UnityEngine;

namespace RPG.UI.InGame
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private Health _health = null;
        [SerializeField] private Canvas healthBarCanvas = null;
        [SerializeField] private RectTransform healthBarFade = null;
        [SerializeField] private RectTransform healthBar = null;
        [SerializeField] private float slowFactor = 0.1f;

        public void UpdateHealthBar(float fraction)
        {
            if (Mathf.Approximately(fraction,0))
            {
                healthBarCanvas.enabled = false;
                return;
            }
            StartCoroutine(UpdateHealth(fraction));
        }

        IEnumerator UpdateHealth(float fraction)
        {
            healthBar.localScale = new Vector3(fraction, 1, 1);

            // healthBarFade.localScale = Mathf.Lerp(healthBarFade.localScale.x, healthBar.localScale.x, slowFactor);
            while (healthBarFade.localScale.x - healthBar.localScale.x > 0.001f)
            {
                //注意！！！这里用的插值healthBarFade.localScale是在无限接近healthBar.localScale
                //所以healthBarFade.localScale永远也不可能小于healthBar.localScale，所以循环无法
                //跳出！！！
                healthBarFade.localScale = Vector3.Lerp(healthBarFade.localScale, healthBar.localScale, slowFactor);
                yield return null;
            }
        }
    }
}
