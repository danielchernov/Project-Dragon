using RPG.Saving;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable, IJsonSaveable
    {
        [SerializeField]
        private float _currentHealth = 100f;

        private bool _isAlreadyDead = false;

        private void Start()
        {
            _currentHealth = GetComponent<BaseStats>().GetHealth();
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
            return 100 * _currentHealth / GetComponent<BaseStats>().GetHealth();
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

            experience.GainExperience(GetComponent<BaseStats>().GetXP());
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
