using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField]
        float _experiencePoints = 0;

        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
        }

        public float GetExperience()
        {
            return _experiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            _experiencePoints = state.ToObject<float>();
        }
    }
}
