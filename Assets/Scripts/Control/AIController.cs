using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private float _chaseDistance = 5f;

        private GameObject _player;
        private Fighter _fighter;
        private Mover _mover;
        private Health _health;

        float _distance;
        Vector3 _guardLocation;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();

            _guardLocation = transform.position;
        }

        private void Update()
        {
            if (_health.IsDead())
                return;

            _distance = Vector3.Distance(_player.transform.position, transform.position);

            if (_distance < _chaseDistance && _fighter.CanAttack(_player))
            {
                _fighter.Attack(_player);
            }
            else
            {
                _mover.StartMoveAction(_guardLocation);
            }
        }

        // Called by Unity
        private void OnDrawGizmos()
        {
            if (_distance < _chaseDistance)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}
