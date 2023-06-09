using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour
    {
        [SerializeField]
        float _experiencePoints = 0;

        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
        }
    }
}
