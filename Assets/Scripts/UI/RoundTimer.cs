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
                    roundTimer.text = "∞";
                }
                else {
                    roundTimer.text = Mathf.CeilToInt(time).ToString();
                }
            }

            //public void Update() {
            //
            //}
        }
    }
}