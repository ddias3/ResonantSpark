﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Walk : BaseState {

            private Vector3 movementForce = Vector3.zero;

            public new void Start() {
                base.Start();
                states.Register(this, "walk");
            }

            public override void Enter(int frameIndex, State previousState) {
                continueInputSearch = true;
                fgChar.Play("walk_forward", 0, 0.0f);
            }

            public override void Execute(int frameIndex, Action<State> changeState) {
                var inputCombos = fgChar.GetFoundCombinations();
                for (int n = 0; n < inputCombos.Count && continueInputSearch; ++n) {
                    Combination combo = inputCombos[n];

                    if (combo.GetType() == typeof(DirectionPress)) OnInput((DirectionPress) combo);
                    else if (combo.GetType() == typeof(DoubleTap)) OnInput((DoubleTap) combo, changeState);
                    else if (combo.GetType() == typeof(NeutralReturn)) OnInput((NeutralReturn) combo, changeState);
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnInput(DirectionPress dirPress) {

            }

            private void OnInput(DoubleTap doubleTap, Action<State> changeState) {
                if (!doubleTap.Stale(frame.index)) {
                    continueInputSearch = false;
                    changeState(states.Get("run"));
                }
            }

            private void OnInput(NeutralReturn doubleTap, Action<State> changeState) {
                if (!doubleTap.Stale(frame.index)) {
                    continueInputSearch = false;
                    changeState(states.Get("idle"));
                }
            }
        }
    }
}