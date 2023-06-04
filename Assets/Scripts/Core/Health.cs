using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable, IJsonSaveable
    {
        [SerializeField]
        private float _currentHealth = 100f;

        private bool _isAlreadyDead = false;

        public bool IsDead()
        {
            return _isAlreadyDead;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);

            if (_currentHealth == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isAlreadyDead)
                return;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            _isAlreadyDead = true;
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
