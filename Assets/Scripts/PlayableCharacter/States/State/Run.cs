using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Run : BaseState {

            public new void Start() {
                base.Start();
                states.Register(this, "run");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    //.On<DoubleTap>(OnDoubleTap)
                    .On<NeutralReturn>(OnNeutralReturn);
            }

            public override void Enter(int frameIndex, IState previousState) {
                Combination combo = messages.Dequeue();
                combo.inUse = false;

                //fgChar.Play("run_forward", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnNeutralReturn(Action stop, Combination combo) {
                if (!combo.Stale(frame.index)) {
                    combo.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("idle").Message(combo));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress) combo;
                if (!dirPress.Stale(frame.index)) {
                    dirPress.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("walk").Message(combo));
                }
            }

            //private void OnDoubleTap(Action stop, Combination combo) {
            //    var doubleTap = (DoubleTap) combo;
            //    if (!doubleTap.Stale(frame.index)) {
            //        doubleTap.inUse = true;
            //        stop.Invoke();
            //        changeState(states.Get("run").Message(combo));
            //    }
            //}
        }
    }
}