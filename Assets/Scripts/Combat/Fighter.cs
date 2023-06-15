using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
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
        private WeaponConfig _defaultWeapon = null;

        WeaponConfig _currentWeaponConfig;
        LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (target == null)
                return;
            if (target.IsDead())
                return;

            if (!IsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, _attackSpeedFraction);
            }
            else
            {
                GetComponent<Mover>().Cancel();

                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator anim = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransform, _leftHandTransform, anim);
        }

        public Health GetTarget()
        {
            return target;
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

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }

            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(
                    _rightHandTransform,
                    _leftHandTransform,
                    target,
                    gameObject,
                    damage
                );
            }
            else
            {
                target.TakeDamage(gameObject, damage);
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.transform.position)
                < _currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
                return false;

            if (
                !GetComponent<Mover>().CanMoveTo(combatTarget.transform.position)
                && !IsInRange(combatTarget.transform)
            )
                return false;

            Health healthToTest = combatTarget.GetComponent<Health>();
            return healthToTest != null && !healthToTest.IsDead();
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>(state.ToObject<string>());
            EquipWeapon(weapon);
        }
    }
}
