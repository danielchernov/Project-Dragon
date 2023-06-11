using UnityEngine;
using System;

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

        [SerializeField]
        private GameObject levelUpParticle;

        int _currentLevel = 0;

        public event Action onLevelUp;

        private void Start()
        {
            _currentLevel = CalculateLevel();

            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;

                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticle, transform);
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, _characterClass, GetLevel());
        }

        public int GetLevel()
        {
            if (_currentLevel < 1)
            {
                _currentLevel = CalculateLevel();
            }
            return _currentLevel;
        }

        public int CalculateLevel()
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
