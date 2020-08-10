using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;
using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterStates {
        public class ForwardDashLong : CharacterBaseState {

            [Tooltip("The dash length in frames")]
            public int dashLength = 18;
            [Tooltip("These many frames of the start of the dash may not be cancelled into another dash")]
            public int redashDisallowed = 10;
            [Tooltip("These many frames of the start of the dash may not be cancelled into an attack")]
            public int attackDisallowed = 16;
            [Tooltip("These many frames of the start of the dash may not block")]
            public int blockDisallowed = 30;

            public AnimationCurve dashPositionCurve;

            private Utility.AttackTracker tracker;

            private FightingGameInputCodeDir dashDir;
            private FightingGameInputCodeDir currDir;

            public new void Awake() {
                base.Awake();
                states.Register(this, "forwardDashLong");

                RegisterInputCallbacks()
                    //.On<DoubleTap>(OnDoubleTap)
                    .On<ButtonsCurrent>(OnButtonsCurrent)
                    .On<DirectionCurrent>(OnDirectionCurrent)
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press);

                RegisterEnterCallbacks()
                    .On<DirectionPlusButton>(GivenDirectionPlusButton);

                tracker = new Utility.AttackTracker();
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Frwd Dash", Color.green);

                dashDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GetInUseCombinations());

                if (dashDir == FightingGameInputCodeDir.Forward) {
                    fgChar.Play("dash_forward"); // fgChar.Play("dash_forward_long");
                }
                else {
                    throw new InvalidOperationException("Entered player state dash without a valid dash direction");
                }

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.AddRelativeVelocity(Gameplay.VelocityPriority.Dash,
                    new Vector3(
                        0.0f,
                        0.0f,
                        AnimationCurveCalculus.Differentiate(dashPositionCurve, tracker.frameCount) / fgChar.gameTime));
                //fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash, animatorVelocity);
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (tracker.frameCount > dashLength) {
                    changeState(states.Get("stand"));
                }

                fgChar.RealignTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
                tracker.Increment();
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

            public override void ReceiveHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                if (forceCrouch) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else {
                    if (currDir == FightingGameInputCodeDir.DownBack) {
                        changeState(states.Get("blockStunCrouch"));
                    }
                    else if (currDir == FightingGameInputCodeDir.Back) {
                        changeState(states.Get("blockStunStand"));
                    }
                }
            }

            public override void ReceiveGrabbed() {
                changeState(states.Get("grabbed"));
            }

            public override bool CheckBlockSuccess(Hit hit) {
                if (tracker.frameCount < blockDisallowed) {
                    return false;
                }
                else {
                    if (currDir == FightingGameInputCodeDir.DownBack) {
                        return hit.validBlocks.Contains(Character.BlockType.LOW);
                    }
                    else if (currDir == FightingGameInputCodeDir.Back) {
                        return hit.validBlocks.Contains(Character.BlockType.HIGH);
                    }
                    else {
                        return false;
                    }
                }
            }

            //private void OnDoubleTap(Action stop, Combination combo) {
            //    if (tracker.frameCount > redashDisallowed) {
            //        var doubleTap = (DoubleTap)combo;
            //        FightingGameInputCodeDir relDir = fgChar.MapAbsoluteToRelative(doubleTap.direction);

            //        if (relDir == FightingGameInputCodeDir.Forward) {
            //            fgChar.UseCombination(doubleTap);
            //            stop();
            //            changeState(states.Get("forwardDash"));
            //        }
            //        else if (relDir == FightingGameInputCodeDir.Back) {
            //            fgChar.UseCombination(doubleTap);
            //            stop();
            //            changeState(states.Get("backDash"));
            //        }
            //    }
            //}

            private void OnDirectionCurrent(Action stop, Combination combo) {
                this.currDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                if (tracker.frameCount > attackDisallowed) {
                    var buttonPress = (ButtonPress)combo;

                    if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                        fgChar.Use(combo);

                        List<Combination> inputs = new List<Combination>();
                        inputs.Add(buttonPress);
                        fgChar.UseCombination<QuarterCircle>(currDir => {
                            inputs.Add(currDir);
                        });
                        fgChar.UseCombination<DoubleTap>(doubleTap => {
                            inputs.Add(doubleTap);
                        });
                        fgChar.UseCombination<DirectionPress>(currPress => {
                            inputs.Add(currPress);
                        });
                        fgChar.UseCombination<DirectionCurrent>(currDir => {
                            inputs.Add(currDir);
                        });
                        inputs.Sort();

                        fgChar.ChooseAttack(this, null, inputs);
                        stop();
                    }
                    else {
                        // TODO: Figure out what happens when you press D here.
                    }
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                if (tracker.frameCount > attackDisallowed) {
                    var but2Press = (Button2Press)combo;
                    Debug.Log("Crouch received 2 button press");
                }
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                //ButtonsCurrent curr = (ButtonsCurrent)combo;
                //this.upJump = !curr.butD;

                // TODO: figure out if anything should happen before the dash actually ends.
            }

            private void GivenDirectionPlusButton(Action stop, Combination combo) {
                this.dashDir = fgChar.MapAbsoluteToRelative(((DirectionPlusButton)combo).direction);
            }
        }
    }
}