using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Dash : BaseState {

            public new void Start() {
                base.Start();
                states.Register(this, "dash");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, State previousState) {
                throw new System.NotImplementedException();
            }

            public override void Execute(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public override void Exit(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public void OnDirectionPress(Action stop, Combination combo) {

            }

            public void OnDoubleTap(Action stop, Combination combo) {

            }
        }
    }
}
