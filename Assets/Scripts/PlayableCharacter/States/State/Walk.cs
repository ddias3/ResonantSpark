using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Walk : BaseState {

            public Vector3 forceScalar;
            public AnimationCurve forwardAccel;
            public AnimationCurve horizontalAccel;
            public float maxForwardSpeed;
            public float maxBackwardSpeed;
            public float maxHorizontalSpeed;

            private Vector3 charAccel;
            private Vector3 movementForce = Vector3.zero;
            private FightingGameInputCodeDir dirPress = FightingGameInputCodeDir.None;

            private float charRotation;

            [Tooltip("in degrees per frame (1/60 s)")]
            public float maxRotation;

            public new void Start() {
                base.Start();
                states.Register(this, "walk");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap)
                    .On<NeutralReturn>(OnNeutralReturn)
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, IState previousState) {
                DirectionPress dirPress = (DirectionPress) messages.Dequeue();
                this.dirPress = dirPress.direction;

                dirPress.inUse = false;
                charAccel = Vector3.zero;

                fgChar.Play("walk_start", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                WalkCharacter();
                TurnCharacter();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter() {
                //...((FightingGameInputCodeDir)((verticalInput + 1) * 3 + (horizontalInput + 1) + 1));

                int cameraX = (((int) dirPress) - 1) % 3 - 1;
                int cameraZ = (((int) dirPress) - 1) / 3 - 1;

                //Debug.Log("FGInput = " + dirPress + " (" + ((int) dirPress) + ") | Char X = " + charX + ", Char Z = " + charZ);

                Vector3 localVelocity = Quaternion.Inverse(fgChar.rigidbody.rotation) * fgChar.rigidbody.velocity;

                Vector3 localInput = fgChar.CameraToChar(new Vector3(cameraX, 0, cameraZ));

                fgChar.SetLocalMoveDirection(localInput.x, localInput.z);

                movementForce = localInput;
                movementForce.Scale(forceScalar);
                movementForce.Scale(new Vector3(
                    horizontalAccel.Evaluate(localVelocity.x / maxHorizontalSpeed),
                    1.0f,
                    forwardAccel.Evaluate(localVelocity.z / (localVelocity.z > 0 ? maxForwardSpeed : maxBackwardSpeed))
                ));

                fgChar.rigidbody.AddRelativeForce(movementForce);
            }

            private void TurnCharacter() {
                charRotation = 0.0f;

                if (fgChar.Grounded()) {
                    charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                    if (charRotation != 0.0f) {
                        fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up) * fgChar.rigidbody.rotation);
                    }
                }
                else {
                    //TODO: don't turn character while in mid air
                }
            }

            private void OnNeutralReturn(Action stop, Combination combo) {
                var neutRet = (NeutralReturn) combo;
                if (!neutRet.Stale(frame.index)) {
                    neutRet.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("idle").Message(combo));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress) combo;
                if (!dirPress.Stale(frame.index)) {
                    //stop.Invoke();
                    this.dirPress = dirPress.direction;
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                if (!doubleTap.Stale(frame.index)) {
                    doubleTap.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("run").Message(combo));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                if (dirPress != ((DirectionCurrent) combo).direction) {
                    this.dirPress = ((DirectionCurrent)combo).direction;
                }
            }
        }
    }
}