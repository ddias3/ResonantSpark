﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Run : BaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "run");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    //.On<DoubleTap>(OnDoubleTap)
                    .On<NeutralReturn>(OnNeutralReturn);
            }

            public override void Enter(int frameIndex, IState previousState) {
                //fgChar.Play("run_forward", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            private void OnNeutralReturn(Action stop, Combination combo) {
                if (!combo.Stale(frame.index)) {
                    combo.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("idle"));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress) combo;
                if (!dirPress.Stale(frame.index)) {
                    dirPress.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("walk"));
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