using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }

        AnimatePlayer();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        bool hasHit = Physics.Raycast(ray, out hitInfo);

        if (hasHit)
        {
            _playerAgent.SetDestination(hitInfo.point);
        }
    }

    private void AnimatePlayer()
    {
        forwardSpeed = (transform.InverseTransformDirection(_playerAgent.velocity)).z;
        _playerAnim.SetFloat("forwardSpeed", forwardSpeed);

        Debug.Log(forwardSpeed);
    }
}
