using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private float _chaseDistance = 5f;

        [SerializeField]
        private float _suspicionTime = 5f;

        [SerializeField]
        private float _dwellingTime = 2f;

        [SerializeField]
        private PatrolPath _patrolPath;

        [SerializeField]
        private float _waypointTolerance = 1;

        [Range(0, 1)]
        [SerializeField]
        private float _patrolSpeedFraction = 0.5f;

        private GameObject _player;
        private Fighter _fighter;
        private Mover _mover;
        private Health _health;

        private float _distance;
        private Vector3 _guardLocation;

        private int _currentWaypointIndex = 0;

        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceWaypoint = Mathf.Infinity;

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
                _timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceWaypoint += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _fighter.Attack(_player);
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardLocation;

            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    _timeSinceWaypoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceWaypoint > _dwellingTime)
            {
                _mover.StartMoveAction(nextPosition, _patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
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
