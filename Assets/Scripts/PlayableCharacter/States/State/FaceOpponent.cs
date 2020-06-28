using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class FaceOpponent : CharacterBaseState {

            public float maxRotation;
            public float stepAwaySpeed;

            private float charRotation;

            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            public new void Awake() {
                base.Awake();
                states.Register(this, "faceOpponent");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Face Opponent", Color.blue);
                GivenInput(fgChar.GetInUseCombinations());

                fgChar.SetLocalWalkParameters(0.0f, 0.0f);
                fgChar.Play("turn_180");
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                Vector3 localVelocity = fgChar.GetLocalVelocity();

                // Move the character
                StepAway();
                TurnCharacter();
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (Mathf.Abs(fgChar.LookToMoveAngle()) < 2f) {
                    changeState(states.Get("stand"));
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void StepAway() {
                if (Mathf.Abs(fgChar.LookToMoveAngle()) > 90f) {
                    fgChar.AddVelocity(Gameplay.VelocityPriority.Movement, fgChar.TargetDirection() * stepAwaySpeed);
                }
            }

            private void TurnCharacter() {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                    if (charRotation != 0.0f) {
                        fgChar.Rotate(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.gameTime, Vector3.up));
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

            public override void GetHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                this.dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }
        }
    }
}
