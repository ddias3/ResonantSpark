using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterStates {
        public class AttackAirborne : Attack {
            private FightingGameInputCodeBut button;
            private FightingGameInputCodeDir direction;

            private FightingGameInputCodeDir currDir;

            public new void Awake() {
                base.Awake();
                states.Register(this, "attackAirborne");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Attack", Color.red);

                fgChar.StartAttackIfRequired(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.RunAttackFrame();
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                //fgChar.UpdateTarget(); fgChar.UpdateAttackTarget();
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
                return fgChar.GetAttackCharacterVulnerability();
            }

            //public override void BeHit(bool launch) {
            //    CharacterProperties.Attack activeAttack = fgChar.GetCurrentAttack();
            //    if (activeAttack.CounterHit()) {
            //        changeState(states.Get("hitStunAirborne"));
            //    }
            //    else {
            //        changeState(states.Get("hitStunAirborne"));
            //    }
            //}

            public override void ReceiveHit(bool launch) {
                changeState(states.Get("hitStunAirborne"));
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("AttackAirborne is somehow being told to block");
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("There shouldn't be any air grabs right now");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress)combo).button0;

                CharacterProperties.Attack activeAttack = fgChar.GetCurrentAttack();

                if (activeAttack.ChainCancellable()) {
                    if (button != FightingGameInputCodeBut.D) {
                        fgChar.Use(combo);

                        List<Combination> inputs = new List<Combination>();
                        inputs.Add(combo);
                        fgChar.UseCombination<QuarterCircle>(currDir => {
                            inputs.Add(currDir);
                        });
                        fgChar.UseCombination<DoubleTap>(doubleTap => {
                            inputs.Add(doubleTap);
                        });
                        fgChar.UseCombination<DirectionPress>(currPress => {
                            inputs.Add(currPress);
                        });
                        fgChar.UseCombination<DirectionCurrent>(currDir => {
                            inputs.Add(currDir);
                        });
                        inputs.Sort();

                        fgChar.ChooseAttack(this, null, inputs);
                        stop();
                    }
                }
            }
        }
    }
}