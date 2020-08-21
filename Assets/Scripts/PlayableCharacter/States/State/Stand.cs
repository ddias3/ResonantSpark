using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Stand : CharacterBaseState {

            public float maxForwardSpeed;
            public float maxBackwardSpeed;
            public float maxHorizontalSpeed;

            public float velocityChangeDampTime;
            public float movementChangeDampTime;
            public Vector3 deadZone;

            [Tooltip("in degrees per frame (1/60 s)")]
            public float maxRotation;

            private Vector3 smoothedInput = Vector3.zero;
            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            private float charRotation;

            private bool upJump = false;
            private bool resetUpJumpControl = false;

            public new void Awake() {
                base.Awake();
                states.Register(this, "stand");

                RegisterInputCallbacks()
                    .On<DoubleTap>(OnDoubleTap)
                    .On<DirectionPlusButton>(OnDirectionPlusButton)
                    .On<Button2Press>(OnButton2Press)
                    .On<ButtonPress>(OnButtonPress)
                    .On<ButtonsCurrent>(OnButtonsCurrent)
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent)
                    .On<NeutralReturn>(GivenNeutralReturn)
                    .On<Empty>(GivenNothing);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Stand", new Color(0.3f, 0.65f, 0.3f));
                dirPress = FightingGameInputCodeDir.Neutral;

                GivenInput(fgChar.GetInUseCombinations());

                fgChar.AnimationStand();

                smoothedInput = fgChar.RelativeInputToLocal(dirPress, upJump);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                Vector3 localVelocity = fgChar.GetLocalVelocity();
                Vector3 localInput = fgChar.RelativeInputToLocal(dirPress, upJump);

                if (dirPress != FightingGameInputCodeDir.Neutral && Mathf.Abs(fgChar.LookToMoveAngle()) > 45f) {
                    changeState(states.Get("faceOpponent"));
                }

                    // Move the character
                WalkCharacter(localVelocity, localInput);
                TurnCharacter(localInput);
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                fgChar.RealignTarget();
                fgChar.UpdateCharacterMovement();
                fgChar.AnimationWalkVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter(Vector3 localVelocity, Vector3 localInput) {
                smoothedInput = Vector3.Lerp(smoothedInput, localInput, movementChangeDampTime * fgChar.gameTime);

                if (smoothedInput.sqrMagnitude > deadZone.sqrMagnitude) {

                    float zMaxSpeed;
                    if (localVelocity.z > 0) {
                        zMaxSpeed = maxForwardSpeed;
                    }
                    else {
                        zMaxSpeed = maxBackwardSpeed;
                    }

                    Vector3 newVelocity = default;
                    Vector3 velocityTarget = new Vector3 {
                        x = smoothedInput.x * maxHorizontalSpeed,
                        y = smoothedInput.y,
                        z = smoothedInput.z * (smoothedInput.z > 0 ? maxForwardSpeed : maxBackwardSpeed)
                    };

                    if (Vector3.Dot(velocityTarget, localVelocity) > 0) {
                        if (localVelocity.sqrMagnitude > velocityTarget.sqrMagnitude) {
                            // let friction slow us down
                            //Debug.Log("Local Velocity: " + localVelocity.ToString("F3") + " Friction slowing us down");
                        }
                        else {
                            newVelocity = Vector3.Lerp(velocityTarget, localVelocity, velocityChangeDampTime * fgChar.gameTime);
                            fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Movement, newVelocity);
                        }
                    }
                    else {
                        //Debug.Log("Smoothed Input: " + smoothedInput.ToString("F3") + ", velTarget=" + velocityTarget.ToString("F3"));
                        newVelocity = Vector3.Lerp(velocityTarget, localVelocity, movementChangeDampTime * fgChar.gameTime);
                        fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Movement, newVelocity);
                    }

                    //Debug.Log("=====================================================");
                    //Debug.Log("Local Velocity: " + localVelocity.ToString("F3") + ", norm=" + normalizedLocalVelocity.ToString("F3"));
                    //Debug.Log("Smoothed Input: " + smoothedInput.ToString("F3") + ", velTarget=" + velocityTarget.ToString("F3"));
                    //Debug.Log("Changing velocity to: " + newVelocity.ToString("F3"));
                }
            }

            private void TurnCharacter(Vector3 localInput) {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    if (localInput.sqrMagnitude > 0.0f) {
                        charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                        if (charRotation != 0.0f) {
                            //fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.gameTime, Vector3.up) * fgChar.rotation);
                            fgChar.Rotate(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.gameTime, Vector3.up));
                        }
                    }
                }
                else {
                    // TODO: don't turn character while in mid air
                    Debug.LogError("Character not grounded while in 'Stand' character state");
                    changeState(states.Get("airborne"));
                }
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

            private void StateSelectOnUpJump(Action stop, Combination combo) {
                switch (this.dirPress) {
                    case FightingGameInputCodeDir.UpBack:
                    case FightingGameInputCodeDir.Up:
                    case FightingGameInputCodeDir.UpForward:
                        fgChar.Use(combo);
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
                        fgChar.Use(combo);
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
                    fgChar.Use(doubleTap);
                    stop();
                    changeState(states.Get("forwardDash"));
                }
                else if (relDir == FightingGameInputCodeDir.Back) {
                    fgChar.Use(doubleTap);
                    stop();
                    changeState(states.Get("backDash"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                this.dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);
                switch (this.dirPress) {
                    case FightingGameInputCodeDir.Back:
                    case FightingGameInputCodeDir.Forward:
                    case FightingGameInputCodeDir.Neutral:
                        resetUpJumpControl = true;
                        break;
                    default:
                        resetUpJumpControl = false; 
                        break;
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress) combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                    List<Combination> inputs = new List<Combination>();

                    //fgChar.UseCombination<DirectionPress>(currPress => {
                    //    fgChar.Use(currPress);
                    //    inputs.Add(currPress);
                    //});
                    //fgChar.UseCombination<QuarterCircle>(quarter => {
                    //    fgChar.Use(quarter);
                    //    inputs.Add(quarter);
                    //});
                    //fgChar.UseCombination<DoubleTap>(doubleTap => {
                    //    fgChar.Use(doubleTap);
                    //    inputs.Add(doubleTap);
                    //});
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

            private void OnDirectionPlusButton(Action stop, Combination combo) {
                var dirPlusBut = (DirectionPlusButton)combo;

                dirPress = fgChar.MapAbsoluteToRelative(dirPlusBut.direction);

                if (dirPlusBut.button0 == FightingGameInputCodeBut.D) {

                    if (dirPress == FightingGameInputCodeDir.Up || dirPress == FightingGameInputCodeDir.Down) {
                        stop();
                        fgChar.Use(dirPlusBut);
                        changeState(states.Get("dodge"));
                    }
                    //else if (dirPress == FightingGameInputCodeDir.Forward) {
                    //    stop();
                    //    fgChar.Use(dirPlusBut);
                    //    changeState(states.Get("forwardDashLong"));
                    //}
                    else if (dirPress == FightingGameInputCodeDir.Back) {
                        stop();
                        fgChar.Use(dirPlusBut);
                        changeState(states.Get("backDashLong"));
                    }
                }
                else {
                    List<Combination> inputs = new List<Combination>();

                    fgChar.Use(combo);
                    inputs.Add(combo);

                    fgChar.ChooseAttack(this, null, inputs);
                    stop();

                    // TODO: Create a mechanism for a frame 0 action, i.e. run the rest of the stand frame, then run the fist frame of the next action while pausing everything else.
                }
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                ButtonsCurrent curr = (ButtonsCurrent) combo;

                if (resetUpJumpControl) {
                    this.upJump = !curr.butD;
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;

                List<Combination> inputs = new List<Combination>();

                fgChar.UseCombination<DirectionCurrent>(currDir => {
                    fgChar.Use(currDir);
                    inputs.Add(currDir);
                });

                fgChar.Use(combo);
                inputs.Add(but2Press);

                inputs.Sort((Combination a, Combination b) => {
                    return a.GetFrame() - b.GetFrame();
                });

                fgChar.ChooseAttack(this, null, inputs);
                stop();
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
