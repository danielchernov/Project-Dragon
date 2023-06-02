using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent _agent;
        private Animator _anim;
        private float forwardSpeed;
        private Health _health;

        [SerializeField]
        private float _maxSpeed = 5.66f;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            _agent.enabled = !_health.IsDead();

            AnimatePlayer();
        }

        public void Cancel()
        {
            _agent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            MoveTo(destination, speedFraction);

            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _agent.SetDestination(destination);

            if (_agent.isStopped)
                _agent.isStopped = false;
        }

        private void AnimatePlayer()
        {
            forwardSpeed = (transform.InverseTransformDirection(_agent.velocity)).z;
            _anim.SetFloat("forwardSpeed", forwardSpeed);
        }
    }
}
