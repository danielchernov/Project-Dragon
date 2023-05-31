using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        private Health target;
        private float _timeSinceLastAttack = Mathf.Infinity;

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
            if (target.IsDead())
                return;

            if (!IsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();

                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (_timeSinceLastAttack > _attackRate)
            {
                GetComponent<Animator>().ResetTrigger("StopAttack");
                GetComponent<Animator>().SetTrigger("Attack");
                _timeSinceLastAttack = 0;
            }
        }

        // Animation Event
        void Hit()
        {
            if (target != null)
            {
                target.TakeDamage(_weaponDamage);
            }
        }

        public void Cancel()
        {
            target = null;

            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < _weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            return combatTarget != null && !combatTarget.GetComponent<Health>().IsDead();
        }
    }
}
