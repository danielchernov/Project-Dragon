using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        private GameObject _equippedPrefab = null;

        [SerializeField]
        private AnimatorOverrideController _animatorOverride = null;

        [SerializeField]
        private float _weaponRange = 2f;

        [SerializeField]
        private float _weaponDamage = 10f;

        [SerializeField]
        private bool isRightHanded = true;

        [SerializeField]
        private Projectile _projectile = null;

        public void SpawnWeapon(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Animator anim
        )
        {
            if (_equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);

                Instantiate(_equippedPrefab, handTransform);
            }
            if (_animatorOverride != null)
            {
                anim.runtimeAnimatorController = _animatorOverride;
            }
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Transform handTransform = GetTransform(rightHand, leftHand);

            Projectile projectileInstance = Instantiate(
                _projectile,
                handTransform.position,
                Quaternion.identity
            );

            projectileInstance.SetTarget(target, _weaponDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetRange()
        {
            return _weaponRange;
        }
    }
}
