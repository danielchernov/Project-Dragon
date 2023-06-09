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

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, _characterClass, GetLevel());
        }

        public int GetLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null)
                return _startingLevel;

            float currentXP = experience.GetExperience();
            for (
                int level = 1;
                level <= progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);
                level++
            )
            {
                float XPtoLevelUp = progression.GetStat(
                    Stat.ExperienceToLevelUp,
                    _characterClass,
                    level
                );
                if (currentXP < XPtoLevelUp)
                {
                    return level;
                }
            }

            return progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass) + 1;
        }
    }
}
