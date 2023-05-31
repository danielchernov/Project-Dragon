using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent _playerAgent;
        private Animator _playerAnim;
        private float forwardSpeed;

        void Start()
        {
            _playerAgent = GetComponent<NavMeshAgent>();
            _playerAnim = GetComponent<Animator>();
        }

        void Update()
        {
            AnimatePlayer();
        }

        public void Cancel()
        {
            _playerAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination)
        {
            MoveTo(destination);

            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void MoveTo(Vector3 destination)
        {
            _playerAgent.SetDestination(destination);

            if (_playerAgent.isStopped)
                _playerAgent.isStopped = false;
        }

        private void AnimatePlayer()
        {
            forwardSpeed = (transform.InverseTransformDirection(_playerAgent.velocity)).z;
            _playerAnim.SetFloat("forwardSpeed", forwardSpeed);
        }
    }
}
