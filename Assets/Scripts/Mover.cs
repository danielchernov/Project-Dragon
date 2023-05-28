using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    private NavMeshAgent _playerAgent;

    void Start()
    {
        _playerAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
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
}
