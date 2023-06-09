using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField]
        private int _startingLevel = 1;

        [SerializeField]
        private CharacterClass _characterClass;

        [SerializeField]
        private Progression progression;

        public float GetHealth()
        {
            return progression.GetHealth(_characterClass, _startingLevel);
        }

        public float GetXP()
        {
            return 10;
        }
    }
}
