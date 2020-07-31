using System;
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
        public class RecoverAirborne : CharacterBaseState {

            [Tooltip("in degrees per frame (1/60 s)")]
            public float maxRotation;

            public Vector3 gravityExtra;

            private float charRotation;

            public new void Awake() {
                base.Awake();
                states.Register(this, "recoverAirborne");
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Recover", Color.white);

                fgChar.Play("airborne_downward");
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

                TurnCharacter();
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                fgChar.RealignTarget();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void TurnCharacter() {
                charRotation = fgChar.LookToMoveAngle();
                if (Mathf.Abs(charRotation) > 5.0f) {
                    fgChar.Rotate(Quaternion.AngleAxis(Mathf.Clamp(charRotation / fgChar.gameTime, -maxRotation, maxRotation) * fgChar.gameTime, Vector3.up));
                }
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.AIRBORNE;
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = false,
                    throwable = false,
                };
            }

            public override void ReceiveHit(bool launch) {
                throw new InvalidOperationException("Shouldn't be able to be hit while recovering airborne");
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("Shouldn't be able to be block while recovering airborne");
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("A character in block stun is being grabbed");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }
        }
    }
}