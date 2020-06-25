using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Menu {
        public class SoundSliders : MonoBehaviour {
            public string optionKey;

            public TMPro.TMP_Text valueLabel;

            private float value;

            public void OnValueChange(float value) {
                this.value = value;
                Persistence.Get().SetOptionValue(optionKey, value);

                valueLabel.text = value.ToString("F2");
            }

            public void Start() {
                value = Persistence.Get().GetOptionValue(optionKey);

                valueLabel.text = value.ToString("F2");
            }
        }
    }
}