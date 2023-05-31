using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float _maxHealth = 100f;

        [SerializeField]
        private float _currentHealth;

        private bool _isAlreadyDead = false;

        public bool IsDead()
        {
            return _isAlreadyDead;
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
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
            print(this + " Died");
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            _isAlreadyDead = true;
        }
    }
}
