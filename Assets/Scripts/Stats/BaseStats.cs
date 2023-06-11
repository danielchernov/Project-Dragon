using UnityEngine;
using System;
using GameDevTV.Utils;

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

        [SerializeField]
        private bool _shouldUseModifiers = false;

        LazyValue<int> _currentLevel;

        public event Action onLevelUp;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel;

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
            return (GetBaseStat(stat) + GetAdditiveModifier(stat))
                * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, _characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return _currentLevel.value;
        }

        private int CalculateLevel()
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

        private float GetAdditiveModifier(Stat stat)
        {
            if (!_shouldUseModifiers)
                return 0;

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!_shouldUseModifiers)
                return 0;

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }
    }
}
