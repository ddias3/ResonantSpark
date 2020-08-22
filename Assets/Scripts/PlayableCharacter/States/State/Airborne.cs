using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Airborne : CharacterBaseState {

            public Vector3 gravityExtra;
            public int delayBeforeCanAttack;

            private string currAnimation;

            private Utility.AttackTracker tracker;

            public new void Awake() {
                base.Awake();
                states.Register(this, "airborne");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DoubleTap>(OnDoubleTap);

                tracker = new Utility.AttackTracker();
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Airborne", Color.yellow);

                currAnimation = ChooseAirborneAnimation(fgChar.GetLocalVelocity());
                fgChar.Play(currAnimation);

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                if (tracker.frameCount > delayBeforeCanAttack) {
                    FindInput(fgChar.GetFoundCombinations());
                }

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
                tracker.Increment();
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

            public override void ReceiveHit(bool launch) {
                changeState(states.Get("hitStunAirborne"));
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("Airborne is somehow being told to block");
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("There shouldn't be any air grabs right now");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
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
                    List<Combination> inputs = new List<Combination>();
                    fgChar.UseCombination<DirectionCurrent>(currDir => {
                        fgChar.Use(currDir);
                        inputs.Add(currDir);
                    });

                    fgChar.Use(combo);
                    inputs.Add(buttonPress);

                    inputs.Sort((Combination a, Combination b) => {
                        return a.GetFrame() - b.GetFrame();
                    });

                    fgChar.ChooseAttack(this, null, inputs);
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