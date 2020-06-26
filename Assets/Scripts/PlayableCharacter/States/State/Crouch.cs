using System;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Crouch : CharacterBaseState {

            private bool crouchDodge = false;
            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            public new void Awake() {
                base.Awake();
                states.Register(this, "crouch");

                RegisterInputCallbacks()
                    .On<DoubleTap>(OnDoubleTap)
                    .On<ButtonsCurrent>(OnButtonsCurrent)
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent)
                    .On<NeutralReturn>(GivenNeutralReturn)
                    .On<Empty>(GivenNothing);
            }

            public override void Enter(int frameIndex, InGameEntityBaseState previousState) {
                fgChar.__debugSetStateText("Crouch", new Color(0.3f, 0.65f, 0.3f));
                dirPress = FightingGameInputCodeDir.Neutral;

                GivenInput(fgChar.GetInUseCombinations());

                fgChar.AnimationCrouch();
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.UpdateCharacterMovement();
                fgChar.CalculateFinalVelocity();
            }

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void LateExecute(int frameIndex) {
                //fgChar.UpdateCharacterMovement();
                //fgChar.AnimationWalkVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHit(bool launch) {
                if (dirPress == FightingGameInputCodeDir.DownBack) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else if (dirPress == FightingGameInputCodeDir.Back) {
                    changeState(states.Get("blockStunStand"));
                }
                else {
                    if (launch) {
                        changeState(states.Get("hitStunAirborne"));
                    }
                    else {
                        changeState(states.Get("hitStunCrouch"));
                    }
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = combo as DirectionPress;

                if (GameInputUtil.Up(fgChar.MapAbsoluteToRelative(dirPress.direction))) {
                    fgChar.Use(dirPress);
                    stop();
                    changeState(states.Get("jump"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var curr = (DirectionCurrent) combo;
                dirPress = fgChar.MapAbsoluteToRelative(curr.direction);

                if (!GameInputUtil.Down(dirPress)) {
                    changeState(states.Get("stand"));
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress)combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                    FightingGameInputCodeDir direction = FightingGameInputCodeDir.Neutral;
                    fgChar.Use(combo);
                    fgChar.UseCombination<DirectionCurrent>(currDir => {
                        direction = fgChar.MapAbsoluteToRelative(((DirectionCurrent)currDir).direction);
                    });

                    fgChar.ChooseAttack(this, null, buttonPress.button0, direction);
                    stop();
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                Debug.Log("Crouch received 2 button press");
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                FightingGameInputCodeDir relDir = fgChar.MapAbsoluteToRelative(doubleTap.direction);

                if (relDir == FightingGameInputCodeDir.Forward ||
                    relDir == FightingGameInputCodeDir.Back) {
                    fgChar.Use(doubleTap);
                    stop();
                    changeState(states.Get("backDash"));
                }
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                ButtonsCurrent curr = (ButtonsCurrent)combo;

                this.crouchDodge = !curr.butD;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void GivenNeutralReturn(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(Input.FightingGameAbsInputCodeDir.Neutral);
            }

            private void GivenNothing(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(Input.FightingGameAbsInputCodeDir.Neutral);
            }
        }
    }
}