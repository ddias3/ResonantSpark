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

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.Play("dash_forward");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public void OnDirectionPress(Action stop, Combination combo) {
                stop();
            }

            public void OnDoubleTap(Action stop, Combination combo) {
                stop();
            }
        }
    }
}
