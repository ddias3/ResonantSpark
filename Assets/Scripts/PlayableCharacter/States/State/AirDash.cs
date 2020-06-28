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
        public class AirDash : CharacterBaseState {

            public Vector3 dashVelocity;

            private Utility.AttackTracker tracker;

            private Vector3 negativeGravity = new Vector3(0f, 9.81f, 0f);

            private FightingGameInputCodeDir dashDir;

            public new void Awake() {
                base.Awake();
                states.Register(this, "airDash");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press);

                RegisterEnterCallbacks()
                    .On<DoubleTap>(GivenDoubleTap)
                    .On<DirectionCurrent>(GivenDirectionCurrent);

                tracker = new Utility.AttackTracker(8);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Air Dash", Color.yellow);
                GivenInput(fgChar.GetInUseCombinations());

                dashDir = FightingGameInputCodeDir.Forward;

                fgChar.Play("airborne_peak");

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (tracker.frameCount < 4) {
                    fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.MovementOverride, Vector3.zero);
                }
                else if (tracker.frameCount == 4) {
                    Vector3 localVelocity = fgChar.GetLocalVelocity();

                    fgChar.AddRelativeVelocity(Gameplay.VelocityPriority.Dash, dashVelocity);
                }
                else {
                    fgChar.AddForce(negativeGravity, ForceMode.Acceleration);
                }
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.UpdateTarget();
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (tracker.frameCount >= 10) {
                    changeState(states.Get("airborne"));
                }
                tracker.Increment();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.AIRBORNE;
            }

            public override void GetHit(bool launch) {
                changeState(states.Get("hitStunAirborne"));
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

            private void GivenDoubleTap(Action stop, Combination combo) {
                dashDir = fgChar.MapAbsoluteToRelative(((DoubleTap)combo).direction);
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                dashDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }
        }
    }
}