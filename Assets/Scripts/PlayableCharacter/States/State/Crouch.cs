using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

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
                    .On<DirectionCurrent>(OnDirectionCurrent)
                    .On<QuarterCircleButtonPress>(OnQuaterCircleButtonPress);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent)
                    .On<NeutralReturn>(GivenNeutralReturn)
                    .On<Empty>(GivenNothing);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
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
                    if (dirPress == FightingGameInputCodeDir.DownBack) {
                        changeState(states.Get("blockStunCrouch"));
                    }
                    else if (dirPress == FightingGameInputCodeDir.Back) {
                        changeState(states.Get("blockStunStand"));
                    }
                }
            }

            public override void ReceiveGrabbed() {
                changeState(states.Get("grabbed"));
            }

            public override bool CheckBlockSuccess(Hit hit) {
                if (dirPress == FightingGameInputCodeDir.DownBack) {
                    return hit.validBlocks.Contains(Character.BlockType.LOW);
                }
                else if (dirPress == FightingGameInputCodeDir.Back) {
                    return hit.validBlocks.Contains(Character.BlockType.HIGH);
                }
                else {
                    return false;
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
                    List<Combination> inputs = new List<Combination>();
                    fgChar.UseCombination<DirectionCurrent>(currDir => {
                        fgChar.Use(currDir);
                        inputs.Add(currDir);
                    });

                    fgChar.Use(combo);
                    inputs.Add(buttonPress);

                    inputs.Sort((Combination a, Combination b) => {
                        return a.GetFrame() - b.GetFrame();
                    });

                    fgChar.ChooseAttack(this, null, inputs);
                    stop();

                    // TODO: Create a mechanism for a frame 0 action, i.e. run the rest of the stand frame, then run the fist frame of the next action while pausing everything else.
                }
            }

            private void OnQuaterCircleButtonPress(Action stop, Combination combo) {
                var quarterBut = (QuarterCircleButtonPress)combo;

                List<Combination> inputs = new List<Combination>();
                fgChar.Use(combo);
                inputs.Add(combo);
                fgChar.ChooseAttack(this, null, inputs);
                stop();
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