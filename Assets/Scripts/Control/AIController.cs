using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private float _chaseDistance = 5f;

        [SerializeField]
        private float _suspicionTime = 3f;

        [SerializeField]
        private float _aggroTime = 5f;

        [SerializeField]
        private float _dwellingTime = 2f;

        [SerializeField]
        private PatrolPath _patrolPath;

        [SerializeField]
        private float _waypointTolerance = 1;

        [SerializeField]
        private float _shoutDistance = 5;

        [Range(0, 1)]
        [SerializeField]
        private float _patrolSpeedFraction = 0.5f;

        private GameObject _player;
        private Fighter _fighter;
        private Mover _mover;
        private Health _health;

        LazyValue<Vector3> _guardLocation;

        private int _currentWaypointIndex = 0;

        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceWaypoint = Mathf.Infinity;
        private float _timeSinceAggro = Mathf.Infinity;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();

            _guardLocation = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardLocation.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead())
                return;

            if (IsAggravated() && _fighter.CanAttack(_player))
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

        public void Aggravate()
        {
            _timeSinceAggro = 0;
        }

        private bool IsAggravated()
        {
            float distance = Vector3.Distance(_player.transform.position, transform.position);

            return distance < _chaseDistance || _timeSinceAggro < _aggroTime;
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceWaypoint += Time.deltaTime;
            _timeSinceAggro += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _fighter.Attack(_player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                _shoutDistance,
                Vector3.up,
                0
            );

            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();

                if (ai != null)
                {
                    ai.Aggravate();
                }
            }
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardLocation.value;

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
        // private void OnDrawGizmos()
        // {
        //     if (distance < _chaseDistance)
        //     {
        //         Gizmos.color = Color.red;
        //     }
        //     else
        //     {
        //         Gizmos.color = Color.blue;
        //     }
        //     Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        // }
    }
}
