using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Dodge : CharacterBaseState {

            [Tooltip("The dodge length in frames")]
            public int dodgeLength = 30;
            [Tooltip("These many frames of the start of the dodge may not be cancelled into another dodge")]
            public int redashDisallowed = 28;
            [Tooltip("These many frames of the start of the dodge may not be cancelled into an attack")]
            public int attackDisallowed = 24;

            public int trackingStart = 6;
            public int trackingEnd = 20;

            public int maxRotation;

            public AnimationCurve dodgeSpeedCurve;

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

                tracker = new Utility.AttackTracker(dodgeLength);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Dodge", Color.green);

                dodgeDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GivenCombinations());

                if (dodgeDir == FightingGameInputCodeDir.Up) {
                    localScaling = new Vector3(-1.0f, 0.0f, 0.0f);
                    fgChar.Play("step_chest");
                }
                else {
                    localScaling = new Vector3(1.0f, 0.0f, 0.0f);
                    fgChar.Play("step_spine");
                }

                tracker.Track(frameIndex);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                Vector3 localVelocity = new Vector3(localScaling.x * dodgeSpeedCurve.Evaluate(tracker.frameCount), 0.0f, 0.0f);

                fgChar.AddRelativeVelocity(Gameplay.VelocityPriority.Dash, localVelocity);

                if (tracker.frameCount > dodgeLength) {
                    changeState(states.Get("stand"));
                }

                if (tracker.frameCount >= trackingStart && tracker.frameCount < trackingEnd) {
                    TurnCharacter(localVelocity);
                }

                fgChar.CalculateFinalVelocity();

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
                            //fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up) * fgChar.rotation);
                            fgChar.Rotate(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up));
                        }
                    }
                }
                else {
                    //TODO: don't turn character while in mid air
                    Debug.LogError("Character not grounded while in 'Stand' character state");
                }
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
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