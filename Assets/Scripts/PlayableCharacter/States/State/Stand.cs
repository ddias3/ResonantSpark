using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Stand : BaseState {

            public Walk walk;
            public WalkSlow walkSlow;
            public Still still;

            public float maxForwardForce;
            public float maxBackwardForce;
            public float maxHorizontalForce;

            public AnimationCurve forwardForce;
            public AnimationCurve horizontalForce;

            public float movementChangeDampTime;
            public Vector3 deadZone;

            [Tooltip("in degrees per frame (1/60 s)")]
            public float maxRotation;

            private Vector3 smoothedInput = Vector3.zero;
            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            private float charRotation;

            private bool upJump = false;

            public new void Awake() {
                base.Awake();
                states.Register(this, "stand");

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

            public override void Enter(int frameIndex, IState previousState) {
                dirPress = FightingGameInputCodeDir.Neutral;

                GivenInput(fgChar.GivenCombinations());

                //int forwardBackward = (((int) dirPress) - 1) % 3 - 1;
                //int upDown          = (((int) dirPress) - 1) / 3 - 1;

                //smoothedInput = fgChar.CameraToWorld(new Vector3(forwardBackward, 0, upJump ? 0 : upDown));
                smoothedInput = fgChar.RelativeInputToLocal(dirPress, upJump);

                fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                fgChar.Play("stand");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                //...((FightingGameInputCodeDir)((verticalInput + 1) * 3 + (horizontalInput + 1) + 1));

                //Debug.Log("FGInput = " + dirPress + " (" + ((int) dirPress) + ") | Char X = " + charX + ", Char Z = " + charZ);

                Vector3 localVelocity = fgChar.GetLocalVelocity();
                Vector3 localInput = fgChar.RelativeInputToLocal(dirPress, upJump);

                    // Move the character
                WalkCharacter(localVelocity, localInput);
                TurnCharacter(localInput);

                    // Use helper states to animate the character
                AnimateCharacter(localVelocity, localInput);
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter(Vector3 localVelocity, Vector3 localInput) {
                smoothedInput = Vector3.Lerp(smoothedInput, localInput, movementChangeDampTime * fgChar.gameTime);

                if (smoothedInput.sqrMagnitude > deadZone.sqrMagnitude) {

                    Vector3 movementForce = new Vector3 {
                        x = smoothedInput.x,
                        y = smoothedInput.y,
                        z = smoothedInput.z,
                    };
                    movementForce.Scale(new Vector3 {
                        x = maxHorizontalForce * horizontalForce.Evaluate(localVelocity.x),
                        y = 1.0f,
                        z = (localVelocity.z > 0 ? maxForwardForce : maxBackwardForce) * forwardForce.Evaluate(localVelocity.z)
                    });

                    //Debug.Log("Local Velocity: " + localVelocity.ToString("F3"));
                    //Debug.Log("Smoothed Input: " + smoothedInput.ToString("F3"));
                    //Debug.Log("Adding local force to character: " + movementForce.ToString("F3"));
                    fgChar.rigidbody.AddRelativeForce(movementForce);
                }
            }

            private void TurnCharacter(Vector3 localInput) {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    if (localInput.sqrMagnitude > 0.0f) {
                        charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                        if (charRotation != 0.0f) {
                            fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up) * fgChar.rigidbody.rotation);
                        }
                    }
                }
                else {
                    //TODO: don't turn character while in mid air
                    Debug.LogError("Character not grounded while in 'Stand' character state");
                }
            }

            private void AnimateCharacter(Vector3 localVelocity, Vector3 localInput) {

            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            private void StateSelectOnUpJump(Action stop, Combination combo) {
                switch (this.dirPress) {
                    case FightingGameInputCodeDir.UpBack:
                    case FightingGameInputCodeDir.Up:
                    case FightingGameInputCodeDir.UpForward:
                        fgChar.UseCombination(combo);
                        stop();
                        changeState(states.Get("jump"));
                        break;
                    case FightingGameInputCodeDir.Back:
                    case FightingGameInputCodeDir.Forward:
                        stop();
                        break;
                    case FightingGameInputCodeDir.DownBack:
                    case FightingGameInputCodeDir.Down:
                    case FightingGameInputCodeDir.DownForward:
                        fgChar.UseCombination(combo);
                        stop();
                        changeState(states.Get("crouch"));
                        break;
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                stop();
                this.dirPress = fgChar.MapAbsoluteToRelative(((DirectionPress) combo).direction);
                if (upJump) {
                    StateSelectOnUpJump(stop, combo);
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap)combo;
                FightingGameInputCodeDir relDir = fgChar.MapAbsoluteToRelative(doubleTap.direction);

                if (relDir == FightingGameInputCodeDir.Forward) {
                    fgChar.UseCombination(doubleTap);
                    stop();
                    changeState(states.Get("forwardDash"));
                }
                else if (relDir == FightingGameInputCodeDir.Back) {
                    fgChar.UseCombination(doubleTap);
                    stop();
                    changeState(states.Get("backDash"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                //Debug.Log(((DirectionCurrent)combo).direction);
                this.dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);
                if (upJump) {
                    StateSelectOnUpJump(stop, combo);
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress) combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                        // TODO: I need to change the input buffer to look further into the future than the input delay for a direction press.
                    fgChar.ChooseAttack(this, null, buttonPress.button0, this.dirPress);
                    stop();
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                Debug.Log("Crouch received 2 button press");
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                ButtonsCurrent curr = (ButtonsCurrent) combo;

                this.upJump = !curr.butD;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionPress) combo).direction);
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);
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
