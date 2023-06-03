using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Fader fader = FindObjectOfType<Fader>();

            DontDestroyOnLoad(gameObject);

            yield return fader.FadeIn(_fadeInTime);

            yield return SceneManager.LoadSceneAsync(_sceneIndex);

            Portal otherPortal = GetOtherPortal();
            SpawnPlayer(otherPortal);

            yield return new WaitForSeconds(_fadeWaitTime);
            yield return fader.FadeOut(_fadeOutTime);

            Destroy(gameObject);
        }

        private void SpawnPlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = otherPortal._spawnPoint.position;
            player.transform.rotation = otherPortal._spawnPoint.rotation;
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
