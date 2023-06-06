using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _arrowSpeed = 100;

        private Health _target;
        private float _damage = 0;

        private void Update()
        {
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * Time.deltaTime * _arrowSpeed);
        }

        public void SetTarget(Health newTarget, float damage)
        {
            _target = newTarget;
            _damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return _target.transform.position;

            return _target.transform.position + Vector3.up * targetCapsule.height * 0.7f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() == _target)
            {
                _target.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
