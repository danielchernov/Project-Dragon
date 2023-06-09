using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }

        [SerializeField]
        private int _sceneIndex = 0;

        [SerializeField]
        private float _fadeInTime = 1f;

        [SerializeField]
        private float _fadeOutTime = 1f;

        [SerializeField]
        private float _fadeWaitTime = 1f;

        [SerializeField]
        private DestinationIdentifier _destination;

        [SerializeField]
        private Transform _spawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

            PlayerController playerController = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeIn(_fadeInTime);

            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneIndex);

            PlayerController newPlayerController = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            SpawnPlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(_fadeWaitTime);
            fader.FadeOut(_fadeOutTime);

            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

        private void SpawnPlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal._spawnPoint.position;
            player.transform.rotation = otherPortal._spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this)
                    continue;

                if (portal._destination != _destination)
                    continue;

                return portal;
            }

            return null;
        }
    }
}
