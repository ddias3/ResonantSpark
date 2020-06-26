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

            public override void Enter(int frameIndex, InGameEntityBaseState previousState) {
                fgChar.__debugSetStateText("Airborne", Color.yellow);

                fgChar.Play("airborne");
            }

            public override void ExecutePass0(int frameIndex) {
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

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void LateExecute(int frameIndex) {
                //fgChar.UpdateCharacterMovement();
                //fgChar.AnimationWalkVelocity();
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

            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap)combo;
                
            }
        }
    }
}