using UnityEngine;

namespace RPG.Attributes
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private RectTransform foreground;

        [SerializeField]
        private Canvas rootCanvas;

        private void Update()
        {
            if (
                Mathf.Approximately(health.GetHealthPoints(), health.GetMaxHealthPoints())
                || Mathf.Approximately(health.GetHealthPoints(), 0)
            )
            {
                rootCanvas.enabled = false;
            }
            else
            {
                rootCanvas.enabled = true;
                foreground.localScale = new Vector3(
                    Mathf.InverseLerp(0, health.GetMaxHealthPoints(), health.GetHealthPoints()),
                    1,
                    1
                );
            }
        }
    }
}
