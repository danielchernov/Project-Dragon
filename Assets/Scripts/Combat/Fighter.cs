using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        public Transform target { get; private set; }

        [SerializeField]
        private float weaponRange = 2f;

        private void Update()
        {
            if (target == null)
                return;

            if (!IsInRange())
            {
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                GetComponent<Mover>().Stop();
            }
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            target = combatTarget.transform;
        }

        public void CancelAttack()
        {
            target = null;
        }
    }
}
