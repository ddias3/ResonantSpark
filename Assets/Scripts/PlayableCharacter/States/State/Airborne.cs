using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Airborne : CharacterBaseState {

            public Vector3 gravityExtra;

            public new void Awake() {
                base.Awake();
                states.Register(this, "airborne");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Airborne", Color.yellow);

                fgChar.Play("airborne");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.AddForce(gravityExtra, ForceMode.Acceleration);

                if (fgChar.Grounded(out Vector3 landPoint)) {
                    changeState(states.Get("land"));
                }

                if (fgChar.CheckAboutToLand()) {
                    changeState(states.Get("land"));
                }

                fgChar.CalculateFinalVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.AIRBORNE;
            }

            public override void GetHitBy(HitBox hitBox) {
                changeState(states.Get("hitStunAirborne"));
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var butPress = (ButtonPress)combo;

            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap)combo;
                
            }
        }
    }
}