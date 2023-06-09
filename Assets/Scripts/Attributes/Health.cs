using RPG.Saving;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable, IJsonSaveable
    {
        private float _currentHealth = -1f;

        private bool _isAlreadyDead = false;

        private void Start()
        {
            if (_currentHealth < 0)
                _currentHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return _isAlreadyDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);

            if (_currentHealth == 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetPercentage()
        {
            return 100 * _currentHealth / GetComponent<BaseStats>().GetStat(Stat.Health);
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

        public object CaptureState()
        {
            return _currentHealth;
        }

        public void RestoreState(object state)
        {
            _currentHealth = (float)state;
            if (_currentHealth == 0)
            {
                Die();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentHealth);
        }

        public void RestoreFromJToken(JToken state)
        {
            _currentHealth = state.ToObject<float>();

            if (_currentHealth == 0)
            {
                Die();
            }
        }
    }
}
