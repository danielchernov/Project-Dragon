using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;

        public void StartAction(IAction action)
        {
            if (_currentAction != null && _currentAction != action)
            {
                Debug.Log("Cancelled " + _currentAction);

                _currentAction.Cancel();

                Debug.Log("Started " + action);
            }
            _currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
