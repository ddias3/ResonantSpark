using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Walk : BaseState {

            public new void Start() {
                base.Start();
                states.Register(this, "walk");
            }

            public override void Enter(State previousState) {
                fgChar.Play("idle", 0, 0.0f);
            }

            public override void Execute(Action<State> changeState) {
                var inputCombos = fgChar.GetFoundCombinations();
                for (int n = 0; n < inputCombos.Count; ++n) {
                    Combination combo = inputCombos[n];

                    if (combo.GetType() == typeof(DirectionPress)) OnInput((DirectionPress)combo);
                    else if (combo.GetType() == typeof(DoubleTap)) OnInput((DoubleTap)combo);
                    else if (combo.GetType() == typeof(NeutralReturn)) OnInput((NeutralReturn)combo);
                }
            }

            public override void Exit() {
                // do nothing
            }

            private void OnInput(DirectionPress dirPress) {

            }

            private void OnInput(DoubleTap doubleTap) {

            }

            private void OnInput(NeutralReturn doubleTap) {

            }
        }
    }
}