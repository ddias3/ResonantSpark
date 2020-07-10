using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Land : CharacterBaseState {

            private int startFrame;
            private int frameCount = 0;

            private FightingGameInputCodeDir dirCurr;

            public new void Awake() {
                base.Awake();
                states.Register(this, "land");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Land", Color.cyan);

                fgChar.GetInUseCombinations();
                fgChar.Play("landing");

                startFrame = frameIndex;
                frameCount = 0;
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (frameCount >= 4) {
                    changeState(states.Get("stand"));
                }

            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.RealignTarget();
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = true,
                };
            }

            public override void GetHit(bool launch) {
                if (dirCurr == FightingGameInputCodeDir.DownBack) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else if (dirCurr == FightingGameInputCodeDir.Down) {
                    changeState(states.Get("blockStunStand"));
                }
                else {
                    if (launch) {
                        changeState(states.Get("hitStunAirborne"));
                    }
                    else {
                        changeState(states.Get("hitStunStand"));
                    }
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var dirPress = (DirectionCurrent) combo;
                dirCurr = fgChar.MapAbsoluteToRelative(dirPress.direction);
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