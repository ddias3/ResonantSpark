using System;
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

            public override void SetValue(string field) {
                switch (field) {
                    case "noTime":
                        SetNoTime();
                        break;
                    default:
                        throw new InvalidOperationException("Round timer field invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0) {
                switch (field) {
                    case "time":
                        SetTime((float) value0);
                        break;
                    default:
                        throw new InvalidOperationException("Round timer field with 1 value invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0, object value1) {
                switch (field) {
                    default:
                        throw new InvalidOperationException("Round timer field with 2 values invalid: " + field);
                }
            }
        }
    }
}