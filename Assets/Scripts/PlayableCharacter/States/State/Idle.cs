using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Idle : BaseState {

            public new void Start() {
                base.Start();
                states.Register(this, "idle");
            }

            public override void Enter(int frameIndex, State previousState) {
                continueInputSearch = true;
                fgChar.Play("idle", 0, 0.0f);
            }

            public override void Execute(int frameIndex, Action<State> changeState) {
                var inputCombos = fgChar.GetFoundCombinations();
                for (int n = 0; n < inputCombos.Count && continueInputSearch; ++n) {
                    Combination combo = inputCombos[n];

                    if (combo.GetType() == typeof(DirectionPress)) OnInput((DirectionPress)combo, changeState);
                    else if (combo.GetType() == typeof(DoubleTap)) OnInput((DoubleTap)combo, changeState);
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnInput(DirectionPress dirPress, Action<State> changeState) {
                if (!dirPress.Stale(frame.index)) {
                    dirPress.inUse = true;
                    continueInputSearch = false;
                    changeState(states.Get("walk"));
                }
            }

            private void OnInput(DoubleTap doubleTap, Action<State> changeState) {
                if (!doubleTap.Stale(frame.index)) {
                    doubleTap.inUse = true;
                    continueInputSearch = false;
                    changeState(states.Get("run"));
                }
            }
        }
    }
}