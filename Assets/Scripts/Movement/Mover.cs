using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
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

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<Fighter>().CancelAttack();
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            _playerAgent.SetDestination(destination);

            if (_playerAgent.isStopped)
                _playerAgent.isStopped = false;
        }

        public void Stop()
        {
            {
                _playerAgent.isStopped = true;
            }
        }

        private void AnimatePlayer()
        {
            forwardSpeed = (transform.InverseTransformDirection(_playerAgent.velocity)).z;
            _playerAnim.SetFloat("forwardSpeed", forwardSpeed);
        }
    }
}
