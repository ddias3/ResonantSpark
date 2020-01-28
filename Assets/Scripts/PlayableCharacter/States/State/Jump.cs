using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Jump : BaseState {

            public Vector3 jump8Impulse;
            public Vector3 jump7Impulse;
            public Vector3 jump9Impulse;
            public Vector3 gravityExtra;

            public float maxSpeed;

            private int startFrame;
            private int frameCount;

            private bool leavingGround;

            private FightingGameInputCodeDir jumpDir;
            private Vector3 jumpImpulse;

            public new void Awake() {
                base.Awake();
                states.Register(this, "jump");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent);
            }

            public override void Enter(int frameIndex, IState previousState) {
                jumpDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GivenCombinations());

                switch (jumpDir) {
                    case FightingGameInputCodeDir.UpBack:
                        Debug.Log("Jump Impulse:(7) " + jumpImpulse);
                        jumpImpulse = jump7Impulse;
                        break;
                    case FightingGameInputCodeDir.Up:
                        Debug.Log("Jump Impulse:(8) " + jumpImpulse);
                        jumpImpulse = jump8Impulse;
                        break;
                    case FightingGameInputCodeDir.UpForward:
                        Debug.Log("Jump Impulse:(9) " + jumpImpulse);
                        jumpImpulse = jump9Impulse;
                        break;
                    default:
                        Debug.Log("Jump didn't have a dirPress");
                        break;
                }
                fgChar.Play("jump_start");
                startFrame = frameIndex;
                frameCount = 0;
                leavingGround = true;
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (frameCount == 4) {
                    Vector3 localVelocity = fgChar.GetLocalVelocity();

                    jumpImpulse.z = jumpImpulse.z * (1 - (localVelocity.z / maxSpeed));

                    fgChar.rigidbody.AddRelativeForce(jumpImpulse, ForceMode.Impulse);
                    fgChar.Play("jump");
                }
                else if (frameCount > 4) {
                    fgChar.rigidbody.AddForce(gravityExtra, ForceMode.Acceleration);
                }

                if (leavingGround) {
                    if (!fgChar.Grounded(out Vector3 standPoint)) {
                        leavingGround = false;
                    }
                }
                else {
                    if (fgChar.Grounded(out Vector3 landPoint)) {
                        changeState(states.Get("land"));
                    }

                    if (fgChar.CheckAboutToLand()) {
                        changeState(states.Get("land"));
                    }
                }

                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var butPress = (ButtonPress)combo;
                if (!butPress.Stale(frame.index)) {
                    Debug.Log("Jump received 1 button press: " + butPress.button0);
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                if (!but2Press.Stale(frame.index)) {
                    Debug.Log("Jump received 2 button press: " + but2Press.button0 + ", " + but2Press.button1);
                }
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                jumpDir = fgChar.MapAbsoluteToRelative(((DirectionPress) combo).direction);
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                jumpDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);
            }
        }
    }
}