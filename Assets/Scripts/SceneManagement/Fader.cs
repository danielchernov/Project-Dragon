using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine routine;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeInImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(0, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(1, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(FadeRoutine(target, time));
            return routine;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(
                    canvasGroup.alpha,
                    target,
                    Time.deltaTime / time
                );
                yield return null;
            }
        }
    }
}
