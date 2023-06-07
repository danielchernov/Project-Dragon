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

        [SerializeField]
        private bool isHoming = false;

        [SerializeField]
        private GameObject hitEffect;

        [SerializeField]
        private float _timeToDie = 10;

        [SerializeField]
        private GameObject[] destroyOnHit;

        [SerializeField]
        private float lifeAfterImpact = 5;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * Time.deltaTime * _arrowSpeed);
        }

        public void SetTarget(Health newTarget, float damage)
        {
            _target = newTarget;
            _damage = damage;

            Destroy(gameObject, _timeToDie);
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
            if (other.GetComponent<Health>() == _target && !_target.IsDead())
            {
                _target.TakeDamage(_damage);

                _arrowSpeed = 0;

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                }

                foreach (GameObject toDestroy in destroyOnHit)
                {
                    Destroy(toDestroy);
                }

                Destroy(gameObject, lifeAfterImpact);
            }
        }
    }
}
