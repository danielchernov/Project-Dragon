using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable, IJsonSaveable
    {
        private NavMeshAgent _agent;
        private Animator _anim;
        private float forwardSpeed;
        private Health _health;

        [SerializeField]
        private float _maxSpeed = 5.66f;

        [SerializeField]
        private float _maxNavPathLength = 40f;

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

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(
                transform.position,
                destination,
                NavMesh.AllAreas,
                path
            );

            if (!hasPath)
                return false;
            if (path.status != NavMeshPathStatus.PathComplete)
                return false;

            if (GetPathLength(path) > _maxNavPathLength)
                return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _agent.SetDestination(destination);

            if (_agent.isStopped)
                _agent.isStopped = false;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0;

            if (path.corners.Length < 2)
                return totalDistance;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return totalDistance;
        }

        private void AnimatePlayer()
        {
            forwardSpeed = (transform.InverseTransformDirection(_agent.velocity)).z;
            _anim.SetFloat("forwardSpeed", forwardSpeed);
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;

            GetComponent<NavMeshAgent>().enabled = false;

            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();

            GetComponent<NavMeshAgent>().enabled = true;
        }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            if (_agent != null)
                _agent.enabled = false;
            transform.position = state.ToVector3();
            if (_agent != null)
                _agent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
