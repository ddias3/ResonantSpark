using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        public class RoundCounter : GameUiElement {
            public List<RectTransform> roundDisplays;
            public Vector3 offset;

            private List<Vector3> positions;

            public new void Start() {
                base.Start();

                positions = new List<Vector3>();
                for (int n = 0; n < roundDisplays.Count; ++n) {
                    positions.Add(roundDisplays[n].position);
                }
            }

            public void SetRoundCount(int numRounds) {
                Debug.Log("Set Round Count : " + numRounds);
                int n;
                for (n = 0; n < numRounds; ++n) {
                    roundDisplays[n].position = positions[n] + offset;
                }
                for (; n < roundDisplays.Count; ++n) {
                    roundDisplays[n].position = positions[n];
                }
            }
        }
    }
}