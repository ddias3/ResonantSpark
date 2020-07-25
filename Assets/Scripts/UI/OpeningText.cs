using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        public class OpeningText : GameUiElement {
            public TextMeshProUGUI mainScreenText;

            public void SetOpeningText(string str) {
                gameObject.SetActive(true);
                mainScreenText.text = str;
            }

            public void Hide() {
                gameObject.SetActive(false);
            }

            public override void SetValue(string field) {
                switch (field) {
                    case "hide":
                        Hide();
                        break;
                    default:
                        throw new InvalidOperationException("Opening text field invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0) {
                switch (field) {
                    case "text":
                        SetOpeningText((string) value0);
                        break;
                    default:
                        throw new InvalidOperationException("Opening text field with 1 value invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0, object value1) {
                switch (field) {
                    default:
                        throw new InvalidOperationException("Opening text field with 2 values invalid: " + field);
                }
            }
        }
    }
}