using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Walk : BaseState {

            private Vector3 movementForce = Vector3.zero;
            private FightingGameInputCodeDir dirPress = FightingGameInputCodeDir.None;

            public new void Start() {
                base.Start();
                states.Register(this, "walk");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap)
                    .On<NeutralReturn>(OnNeutralReturn)
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, State previousState) {
                DirectionPress dirPress = (DirectionPress) messages.Dequeue();
                this.dirPress = dirPress.direction;

                dirPress.inUse = false;

                fgChar.Play("walk_forward", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
                WalkCharacter();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter() {
                if (dirPress == FightingGameInputCodeDir.Right) {
                    //TODO: Move character
                    Debug.Log("Would move character to the right");
                }
            }

            private void OnNeutralReturn(Action stop, Combination combo) {
                var neutRet = (NeutralReturn) combo;
                if (!neutRet.Stale(frame.index)) {
                    neutRet.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("idle").Message(combo));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress) combo;
                if (!dirPress.Stale(frame.index)) {
                    //stop.Invoke();
                    this.dirPress = dirPress.direction;
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                if (!doubleTap.Stale(frame.index)) {
                    doubleTap.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("run").Message(combo));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                if (dirPress != ((DirectionCurrent) combo).direction) {
                    this.dirPress = ((DirectionCurrent)combo).direction;
                }
            }
        }
    }
}