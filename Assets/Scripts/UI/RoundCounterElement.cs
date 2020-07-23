using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ResonantSpark {
    namespace UI {
        public class RoundCounterElement : MonoBehaviour {
            public Image border;
            public Image fill;

            public void Start() {
                DisplayFill(false);
            }

            public void DisplayFill(bool display) {
                fill.enabled = display;
            }
        }
    }
}