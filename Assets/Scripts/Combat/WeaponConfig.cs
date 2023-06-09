using UnityEngine;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField]
        private Weapon _equippedPrefab = null;

        [SerializeField]
        private AnimatorOverrideController _animatorOverride = null;

        [SerializeField]
        private float _weaponRange = 2f;

        [SerializeField]
        private float _weaponDamage = 10f;

        [SerializeField]
        private float _percentageBonus = 0;

        [SerializeField]
        private bool isRightHanded = true;

        [SerializeField]
        private Projectile _projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Animator anim
        )
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            Weapon weapon = null;

            if (_equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);

                weapon = Instantiate(_equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideControler = anim.runtimeAnimatorController as AnimatorOverrideController;

            if (_animatorOverride != null)
            {
                anim.runtimeAnimatorController = _animatorOverride;
            }
            else if (overrideControler != null)
            {
                anim.runtimeAnimatorController = overrideControler.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null)
                return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handTransform;

            if (isRightHanded)
                handTransform = rightHandTransform;
            else
                handTransform = leftHandTransform;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public void LaunchProjectile(
            Transform rightHand,
            Transform leftHand,
            Health target,
            GameObject instigator,
            float calculatedDamage
        )
        {
            Transform handTransform = GetTransform(rightHand, leftHand);

            Projectile projectileInstance = Instantiate(
                _projectile,
                handTransform.position,
                Quaternion.identity
            );

            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return _percentageBonus;
        }

        public float GetRange()
        {
            return _weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _percentageBonus;
            }
        }
    }
}
