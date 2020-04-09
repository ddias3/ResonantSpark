using System.Collections;
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
        }
    }
}