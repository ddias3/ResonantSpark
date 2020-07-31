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
        public class Jump : CharacterBaseState {

            public Vector3 jump8Velocity;
            public Vector3 jump7Velocity;
            public Vector3 jump9Velocity;
            public Vector3 gravityExtra;

            public float maxSpeed;

            private int startFrame;
            private int frameCount;

            private bool leavingGround;

            private FightingGameInputCodeDir jumpDir;
            private Vector3 jumpVelocity;

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

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Jump", Color.yellow);

                jumpDir = FightingGameInputCodeDir.None;
                GivenInput(fgChar.GetInUseCombinations());

                switch (jumpDir) {
                    case FightingGameInputCodeDir.UpBack:
                        Debug.Log("Jump Impulse:(7) " + jumpVelocity);
                        jumpVelocity = jump7Velocity;
                        break;
                    case FightingGameInputCodeDir.Up:
                        Debug.Log("Jump Impulse:(8) " + jumpVelocity);
                        jumpVelocity = jump8Velocity;
                        break;
                    case FightingGameInputCodeDir.UpForward:
                        Debug.Log("Jump Impulse:(9) " + jumpVelocity);
                        jumpVelocity = jump9Velocity;
                        break;
                    default:
                        Debug.Log("Jump didn't have a dirPress");
                        break;
                }
                fgChar.Play("jump");
                startFrame = frameIndex;
                frameCount = 0;
                leavingGround = true;
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (frameCount == 4) {
                    Vector3 localVelocity = fgChar.GetLocalVelocity();

                        // TODO: fix this to be a more useful function.
                    //jumpVelocity.z = jumpVelocity.z * (1 - (localVelocity.z / maxSpeed));
                    jumpVelocity.z = Mathf.Lerp(jumpVelocity.z, localVelocity.z, Mathf.Abs(localVelocity.z) / maxSpeed);

                    fgChar.AddRelativeVelocity(Gameplay.VelocityPriority.Jump, jumpVelocity);
                }
                else if (frameCount > 4) {
                    changeState(states.Get("airborne"));
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
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.RealignTarget();
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                if (frameCount > 4) {
                    return GroundRelation.AIRBORNE;
                }
                else {
                    return GroundRelation.GROUNDED;
                }
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = false,
                };
            }

            public override void ReceiveHit(bool launch) {
                if (frameCount > 4) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    if (launch) {
                        changeState(states.Get("hitStunAirborne"));
                    }
                    else {
                        changeState(states.Get("hitStunStand"));
                    }
                }
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("A character jumping is successfully blocking");
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("A character jumping is being grabbed");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
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