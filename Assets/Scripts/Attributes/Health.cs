using RPG.Saving;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;
using System;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable, ISaveable
    {
        [SerializeField]
        float regenerationPercentage = 70;

        [SerializeField]
        private UnityEvent<float> takeDamage;

        [SerializeField]
        private UnityEvent onDie;

        private LazyValue<float> _currentHealth;

        private bool _isAlreadyDead = false;

        private void Awake()
        {
            _currentHealth = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            _currentHealth.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return _isAlreadyDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            _currentHealth.value = Mathf.Max(_currentHealth.value - damage, 0);

            takeDamage.Invoke(damage);

            if (_currentHealth.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetHealthPoints()
        {
            return _currentHealth.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * _currentHealth.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (_isAlreadyDead)
                return;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            _isAlreadyDead = true;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null)
                return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints =
                GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            _currentHealth.value = Mathf.Max(_currentHealth.value, regenHealthPoints);
        }

        public object CaptureState()
        {
            return _currentHealth;
        }

        public void RestoreState(object state)
        {
            _currentHealth.value = (float)state;
            if (_currentHealth.value == 0)
            {
                Die();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentHealth.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            _currentHealth.value = state.ToObject<float>();

            if (_currentHealth.value == 0)
            {
                Die();
            }
        }

        public void Heal(float healthToRestore)
        {
            _currentHealth.value = Mathf.Min(
                _currentHealth.value + healthToRestore,
                GetMaxHealthPoints()
            );
        }
    }
}
