using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        public class RoundCounter : GameUiElement {
            public List<RoundCounterElement> roundDisplays;
            public Vector3 offset;

            private List<Vector3> positions;

            public new void Start() {
                base.Start();

                positions = new List<Vector3>();
                for (int n = 0; n < roundDisplays.Count; ++n) {
                    positions.Add(roundDisplays[n].transform.position);
                }
            }

            public void SetRoundCount(int numRounds) {
                Debug.Log("Set Round Count : " + numRounds);
                int n;
                for (n = 0; n < numRounds; ++n) {
                    roundDisplays[n].transform.position = positions[n] + offset;
                    roundDisplays[n].DisplayFill(true);
                }
                for (; n < roundDisplays.Count; ++n) {
                    roundDisplays[n].transform.position = positions[n];
                    roundDisplays[n].DisplayFill(false);
                }
            }
        }
    }
}