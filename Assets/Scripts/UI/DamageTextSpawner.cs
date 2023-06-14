using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        private DamageText _damageTextPrefab;

        public void SpawnDamageText(float damageAmount)
        {
            DamageText damageText = Instantiate<DamageText>(_damageTextPrefab, transform);

            damageText.SetValue(damageAmount);
        }
    }
}
