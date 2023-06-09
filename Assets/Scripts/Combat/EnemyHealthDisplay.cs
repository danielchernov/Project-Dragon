using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        string valueToPrint;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTarget() != null)
            {
                valueToPrint = Mathf.Round(fighter.GetTarget().GetPercentage()) + "%";
            }
            else
            {
                valueToPrint = "N/A";
            }

            GetComponent<Text>().text = valueToPrint;
        }
    }
}
