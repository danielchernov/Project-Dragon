using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
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
            if (_currentWeapon == null)
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
            _currentWeapon.Spawn(_rightHandTransform, _leftHandTransform, anim);
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

            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeapon.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            Weapon weapon = Resources.Load<Weapon>(state.ToObject<string>());
            EquipWeapon(weapon);
        }
    }
}
