using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Dodge : CharacterBaseState {

            [Tooltip("The dodge length in frames")]
            public int dodgeLength = 30;
            [Tooltip("These many frames of the start of the dodge may not be cancelled into another dodge")]
            public int redashDisallowed = 28;
            [Tooltip("These many frames of the start of the dodge may not be cancelled into an attack")]
            public int attackDisallowed = 24;
            [Tooltip("These many frames of the start of the dash may not be cancelled into an attack")]
            public int blockDisallowed = 12;

            public AnimationCurve dodgePositionCurve;

            public int trackingStart = 6;
            public int trackingEnd = 20;

            public int maxRotation;

            private Utility.AttackTracker tracker;

            private FightingGameInputCodeDir dodgeDir;
            private Vector3 localScaling;

            public new void Awake() {
                base.Awake();
                states.Register(this, "dodge");

                RegisterInputCallbacks()
                    .On<Button2Press>(OnButton2Press)
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPlusButton>(GivenDirectionPlusButton);

                tracker = new Utility.AttackTracker();
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Dodge", Color.green);

                dodgeDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GetInUseCombinations());

                localScaling = fgChar.RelativeInputToLocal(dodgeDir, false);

                if (localScaling.x > 0) {
                    fgChar.Play("dodge_right");
                }
                else {
                    fgChar.Play("dodge_left");
                }

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                Vector3 localVelocity = new Vector3 {
                    x = 1.0f,
                    y = 0.0f,
                        // TODO: Figure out the actual function here to get to the correct new location
                    z = 0.3f / fgChar.TargetDirection().magnitude,
                };

                //fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash, animatorDelta);

                if (tracker.frameCount > dodgeLength) {
                    changeState(states.Get("stand"));
                }

                fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash,
                    new Vector3(
                        localScaling.x * AnimationCurveCalculus.Differentiate(dodgePositionCurve, tracker.frameCount) / fgChar.gameTime,
                        0.0f,
                        0.0f));
                if (tracker.frameCount >= trackingStart && tracker.frameCount < trackingEnd) {
                    TurnCharacter(localVelocity);
                }
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (tracker.frameCount >= trackingStart && tracker.frameCount < trackingEnd) {
                    fgChar.RealignTarget();
                }
                //fgChar.UpdateCharacterMovement();
                //fgChar.AnimationWalkVelocity();
                tracker.Increment();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void TurnCharacter(Vector3 localInput) {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    if (localInput.sqrMagnitude > 0.0f) {
                        float charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
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
                if (tracker.frameCount < trackingEnd) {
                    return new CharacterVulnerability {
                        strikable = true,
                        throwable = false,
                    };
                }
                else {
                    return new CharacterVulnerability {
                        strikable = true,
                        throwable = true,
                    };
                }
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
                    if (dodgeDir == FightingGameInputCodeDir.DownBack) {
                        changeState(states.Get("blockStunCrouch"));
                    }
                    else if (dodgeDir == FightingGameInputCodeDir.Back) {
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
                    if (dodgeDir == FightingGameInputCodeDir.DownBack) {
                        return hit.validBlocks.Contains(Character.BlockType.LOW);
                    }
                    else if (dodgeDir == FightingGameInputCodeDir.Back) {
                        return hit.validBlocks.Contains(Character.BlockType.HIGH);
                    }
                    else {
                        return false;
                    }
                }
            }

            private void GivenDirectionPlusButton(Action stop, Combination combo) {
                var dirPlusBut = (DirectionPlusButton) combo;

                dodgeDir = fgChar.MapAbsoluteToRelative(dirPlusBut.direction);

                if (dodgeDir == FightingGameInputCodeDir.Up || dodgeDir == FightingGameInputCodeDir.Down) {
                    stop();
                }
                else {
                    throw new InvalidOperationException("Dodge state without pressing up or down");
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                this.dodgeDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress)combo;
                Debug.Log("Dodge received 1 button press");
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                Debug.Log("Dodge received 2 button press");
            }
        }
    }
}