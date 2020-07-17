using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        public class RoundTimer : GameUiElement {
            public TextMeshProUGUI roundTimer;

            public void SetTime(float time) {
                if (time < 0) {
                    roundTimer.text = "00";
                }
                else {
                    roundTimer.text = Mathf.CeilToInt(time).ToString("D2");
                }
            }

            public void SetNoTime() {
                roundTimer.text = "∞";
            }

            //public void Update() {
            //
            //}
        }
    }
}