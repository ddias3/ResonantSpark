using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Idle : BaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "idle");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press)
                    .On<DirectionCurrent>(OnDirectionCurrent)
                    .On<DirectionPlusButton>(OnDirectionPlusButton)
                    .On<DoubleTap>(OnDoubleTap)
                    .On<QuarterCircle>(OnQuarterCircle)
                    .On<QuarterCircleButtonPress>(OnQuarterCircleButtonPress);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.Play("idle");
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

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var dirPress = (DirectionCurrent) combo;
                switch (dirPress.direction) {
                    case FightingGameAbsInputCodeDir.UpLeft:
                    case FightingGameAbsInputCodeDir.Up:
                    case FightingGameAbsInputCodeDir.UpRight:
                        fgChar.UseCombination(dirPress);
                        stop();
                        changeState(states.Get("jump"));
                        break;
                    case FightingGameAbsInputCodeDir.Left:
                    case FightingGameAbsInputCodeDir.Right:
                        fgChar.UseCombination(dirPress);
                        stop();
                        changeState(states.Get("walk"));
                        break;
                    case FightingGameAbsInputCodeDir.DownLeft:
                    case FightingGameAbsInputCodeDir.Down:
                    case FightingGameAbsInputCodeDir.DownRight:
                        fgChar.UseCombination(dirPress);
                        stop();
                        changeState(states.Get("crouch"));
                        break;
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                if (!doubleTap.Stale(frame.index)) {
                    doubleTap.inUse = true;
                    stop();
                    changeState(states.Get("dash"));
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var butPress = (ButtonPress) combo;
                if (!butPress.Stale(frame.index)) {
                    switch (butPress.button0) {
                        case FightingGameInputCodeBut.A:
                            Debug.Log("Idle pressed A");
                            break;
                        case FightingGameInputCodeBut.B:
                            Debug.Log("Idle pressed B");
                            break;
                        case FightingGameInputCodeBut.C:
                            Debug.Log("Idle pressed C");
                            break;
                        case FightingGameInputCodeBut.D:
                            Debug.Log("Idle pressed D");
                            break;
                        case FightingGameInputCodeBut.S:
                            Debug.Log("Idle pressed S");
                            break;
                    }
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press) combo;
                if (!but2Press.Stale(frame.index)) {
                    Debug.Log("Idle received 2 button press");
                }
            }

            private void OnDirectionPlusButton(Action stop, Combination combo) {
                var dirPlusBut = (DirectionPlusButton) combo;
                if (!dirPlusBut.Stale(frame.index)) {
                    Debug.Log("Idle received Direction+Button");
                }
            }

            private void OnQuarterCircle(Action stop, Combination combo) {
                var quartCir = (QuarterCircle) combo;
                if (!quartCir.Stale(frame.index)) {
                    //quartCir.inUse = true;
                    //stop();
                    //changeState(states.Get("walk"));
                    Debug.Log("Idle received Quarter Circle");
                }
            }

            private void OnQuarterCircleButtonPress(Action stop, Combination combo) {
                var qcPlusBut = (QuarterCircleButtonPress) combo;
                if (!qcPlusBut.Stale(frame.index)) {
                    Debug.Log("Idle received QuarterCircle+Button");
                }
            }
        }
    }
}