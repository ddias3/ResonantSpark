using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class ForwardDashLong : CharacterBaseState {

            [Tooltip("The dash length in frames")]
            public int dashLength = 18;
            [Tooltip("These many frames of the start of the dash may not be cancelled into another dash")]
            public int redashDisallowed = 10;
            [Tooltip("These many frames of the start of the dash may not be cancelled into an attack")]
            public int attackDisallowed = 16;

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

                tracker = new Utility.AttackTracker(dashLength);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Frwd Dash", Color.green);

                dashDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GetInUseCombinations());

                fgChar.Play("dash_forward_long");

                tracker.Track(frameIndex);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (tracker.frameCount > dashLength) {
                    changeState(states.Get("stand"));
                }

                fgChar.CalculateFinalVelocity();
                tracker.Increment();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                //distance += animatorDelta / fgChar.gameTime;
                fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash, animatorVelocity);
                //fgChar.position += fgChar.toLocal * animatorDelta;
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHitBy(HitBox hitBox) {
                changeState(states.Get("hitStunStand"));
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
                        fgChar.ChooseAttack(this, null, buttonPress.button0, this.currDir);
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