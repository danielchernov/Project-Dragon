using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        private Text damageText;

        public void SetValue(float value)
        {
            damageText.text = String.Format("{0:0}", value);
        }
    }
}
