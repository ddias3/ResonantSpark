using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Airborne : CharacterBaseState {

            public Vector3 gravityExtra;

            private string currAnimation;

            public new void Awake() {
                base.Awake();
                states.Register(this, "airborne");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Airborne", Color.yellow);

                currAnimation = ChooseAirborneAnimation(fgChar.GetLocalVelocity());
                fgChar.Play(currAnimation);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (currAnimation != ChooseAirborneAnimation(fgChar.GetLocalVelocity())) {
                    currAnimation = ChooseAirborneAnimation(fgChar.GetLocalVelocity());
                    fgChar.Play(currAnimation);
                }

                fgChar.AddForce(gravityExtra, ForceMode.Acceleration);

                if (fgChar.Grounded(out Vector3 landPoint)) {
                    changeState(states.Get("land"));
                }

                if (fgChar.CheckAboutToLand()) {
                    changeState(states.Get("land"));
                }
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

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.AIRBORNE;
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = true,
                };
            }

            public override void GetHit(bool launch) {
                changeState(states.Get("hitStunAirborne"));
            }

            private string ChooseAirborneAnimation(Vector3 velocity) {
                if (velocity.y < -1.0f) {
                    return "airborne_downward";
                }
                else if (velocity.y < 1.0f) {
                    return "airborne_peak";
                }
                else {
                    return "airborne_upward";
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress)combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                    FightingGameInputCodeDir direction = FightingGameInputCodeDir.Neutral;
                    fgChar.Use(combo);
                    fgChar.UseCombination<DirectionCurrent>(currDir => {
                        direction = fgChar.MapAbsoluteToRelative(((DirectionCurrent)currDir).direction);
                    });

                    fgChar.ChooseAttack(this, null, buttonPress.button0, direction);
                    stop();
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap)combo;

                FightingGameInputCodeDir relDir = fgChar.MapAbsoluteToRelative(doubleTap.direction);

                if (relDir == FightingGameInputCodeDir.Forward) {
                    fgChar.Use(doubleTap);
                    stop();
                    changeState(states.Get("airDash"));
                }
            }
        }
    }
}