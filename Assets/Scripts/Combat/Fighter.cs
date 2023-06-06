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
        private float _attackSpeedFraction = 1f;

        [SerializeField]
        private float _attackRate = 1f;

        [SerializeField]
        private Transform _rightHandTransform = null;

        [SerializeField]
        private Transform _leftHandTransform = null;

        [SerializeField]
        private Weapon _defaultWeapon = null;
        private Weapon _currentWeapon = null;

        private void Start()
        {
            EquipWeapon(_defaultWeapon);
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (target == null)
                return;
            if (target.IsDead())
                return;

            if (!IsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, _attackSpeedFraction);
            }
            else
            {
                GetComponent<Mover>().Cancel();

                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            Animator anim = GetComponent<Animator>();
            _currentWeapon.SpawnWeapon(_rightHandTransform, _leftHandTransform, anim);
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

        // Animation Events
        void Hit()
        {
            if (target == null)
                return;

            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(_rightHandTransform, _leftHandTransform, target);
            }
            else
            {
                target.TakeDamage(_currentWeapon.GetDamage());
            }
        }

        void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            target = null;

            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");

            GetComponent<Mover>().Cancel();
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position)
                < _currentWeapon.GetRange();
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
