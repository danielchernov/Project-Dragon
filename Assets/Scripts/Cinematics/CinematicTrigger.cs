using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool cutscenePlayed = false;

        public object CaptureState()
        {
            return cutscenePlayed;
        }

        public void RestoreState(object state)
        {
            cutscenePlayed = (bool)state;
        }

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
