using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ResonantSpark
{
    namespace UI
    {
        public class OpeningText : GameUiElement
        {
            public TextMeshProUGUI roundTimer;

            public void SetOpeningText(string str)
            {
                roundTimer.text = str;
            }

            //public void Update() {
            //
            //}
        }
    }
}