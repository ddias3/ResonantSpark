using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class FaceOpponent : CharacterBaseState {

            public float maxRotation;

            private Vector3 smoothedInput;
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

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Face Opponent", Color.blue);
                GivenInput(fgChar.GivenCombinations());

                //smoothedInput = fgChar.RelativeInputToLocal(dirPress, upJump);

                fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                fgChar.Play("faceOpponent");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                Vector3 localVelocity = fgChar.GetLocalVelocity();
                Vector3 localInput = smoothedInput; // fgChar.RelativeInputToLocal(dirPress, upJump);

                // Move the character
                WalkCharacter(localVelocity, localInput);
                TurnCharacter(localInput);
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter(Vector3 localVelocity, Vector3 localInput) {
                //smoothedInput = Vector3.Lerp(smoothedInput, localInput, movementChangeDampTime * fgChar.gameTime);

                if (smoothedInput.sqrMagnitude > 0) {

                    Vector3 movementForce = new Vector3 {
                        x = smoothedInput.x,
                        y = smoothedInput.y,
                        z = smoothedInput.z,
                    };
                    movementForce.Scale(new Vector3 {
                        x = 1.0f, //maxHorizontalForce * horizontalForce.Evaluate(localVelocity.x),
                        y = 1.0f,
                        z = 1.0f, //(localVelocity.z > 0 ? maxForwardForce : maxBackwardForce) * forwardForce.Evaluate(localVelocity.z)
                    });

                    //fgChar.rigidbody.AddRelativeForce(movementForce);
                }
            }

            private void TurnCharacter(Vector3 localInput) {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    if (localInput.sqrMagnitude > 0.0f) {
                        charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                        if (charRotation != 0.0f) {
                            //fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up) * fgChar.rigidbody.rotation);
                        }
                    }
                }
                else {
                    //TODO: don't turn character while in mid air
                    Debug.LogError("Character not grounded while in 'Stand' character state");
                }
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHitBy(HitBox hitBox) {
                changeState(states.Get("hitStunStand"));
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
