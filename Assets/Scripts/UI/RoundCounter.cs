using System;
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

            public new void Awake() {
                base.Awake();

                positions = new List<Vector3>();
                for (int n = 0; n < roundDisplays.Count; ++n) {
                    positions.Add(roundDisplays[n].transform.position);
                }
            }

            public void SetRoundWins(int numRounds) {
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

            public override void SetValue(string field) {
                switch (field) {
                    default:
                        throw new InvalidOperationException("Round counter field invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0) {
                switch (field) {
                    case "roundWins":
                        SetRoundWins((int) value0);
                        break;
                    default:
                        throw new InvalidOperationException("Round counter field with 1 value invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0, object value1) {
                switch (field) {
                    default:
                        throw new InvalidOperationException("Round counter field with 2 values invalid: " + field);
                }
            }
        }
    }
}