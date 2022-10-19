using System.Collections;
using UnityEngine;

namespace RPG.SceneManger
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private float fadeOutTime = 1.5f;
        private CanvasGroup _canvasGroup;
        private Coroutine currentFadeCoroutine = null;
        
        void Awake()
        {
            // Debug.Log(gameObject + " - " + gameObject.GetHashCode());
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public Coroutine FadeIn(float time)
        {
            return StartFadeCoroutine(0, time);
        }

        public Coroutine FadeOut(float time)
        {
            return StartFadeCoroutine(1, time);
        }

        public Coroutine StartFadeCoroutine(float target, float time)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = StartCoroutine(FadeRoutine(target, time));
            return currentFadeCoroutine;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha,target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
