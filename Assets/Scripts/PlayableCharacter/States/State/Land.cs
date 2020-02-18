using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Land : CharacterBaseState {

            private int startFrame;
            private int frameCount = 0;

            public new void Awake() {
                base.Awake();
                states.Register(this, "land");

                    // See note below
                //RegisterInputCallbacks()
                //    .On<DirectionPress>(OnDirectionPress)
                //    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.GivenCombinations();
                fgChar.Play("jump_land");

                startFrame = frameIndex;
                frameCount = 0;
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (frameCount >= 4) {
                    changeState(states.Get("stand"));
                }

                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            // TODO: Check to make sure this works. I THINK that if these functions are omitted, then the inputs that are in the buffer
            //      will remain there and be present for the next state that can serve them.
            //private void OnDirectionPress(Action stop, Combination combo) {
            //    if (frameCount >= 4) {
            //        var dirPress = (DirectionPress)combo;
            //        stop();
            //        fgChar.UseCombination(combo);
            //        changeState(states.Get("stand"));
            //    }
            //}

            //private void OnDoubleTap(Action stop, Combination combo) {
            //    if (frameCount >= 4) {
            //        var doubleTap = (DoubleTap)combo;
            //        stop();
            //        fgChar.UseCombination(combo);
            //        changeState(states.Get("run"));
            //    }
            //}
        }
    }
}