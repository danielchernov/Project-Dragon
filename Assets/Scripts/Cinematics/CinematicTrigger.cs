using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool cutscenePlayed = false;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player" && !cutscenePlayed)
            {
                GetComponent<PlayableDirector>().Play();
                cutscenePlayed = true;
            }
        }
    }
}
