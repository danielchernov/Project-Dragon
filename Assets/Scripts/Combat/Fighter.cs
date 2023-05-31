using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        public Transform target { get; private set; }
        float _timeSinceLastAttack = 0;

        [SerializeField]
        private float _weaponRange = 2f;

        [SerializeField]
        private float _attackRate = 1f;

        [SerializeField]
        private float _weaponDamage = 10f;

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (target == null)
                return;

            if (!IsInRange())
            {
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();

                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (_timeSinceLastAttack > _attackRate)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                _timeSinceLastAttack = 0;
            }
        }

        // Animation Event
        void Hit()
        {
            target.GetComponent<Health>().TakeDamage(_weaponDamage);
        }

        public void Cancel()
        {
            target = null;
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < _weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            target = combatTarget.transform;
            GetComponent<ActionScheduler>().StartAction(this);
        }
    }
}
