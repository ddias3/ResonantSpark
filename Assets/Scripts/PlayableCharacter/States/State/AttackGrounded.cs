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
        public class AttackGrounded : Attack {

            private InputNotation notation;

            private FightingGameInputCodeBut button;
            private FightingGameInputCodeDir direction;

            private FightingGameInputCodeDir currDir;

            public new void Awake() {
                base.Awake();
                states.Register(this, "attackGrounded");

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
                return GroundRelation.GROUNDED;
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
            //        if (launch) {
            //            changeState(states.Get("hitStunAirborne"));
            //        }
            //        else {
            //            changeState(states.Get("hitStunStand"));
            //        }
            //    }
            //    else {
            //        if (launch) {
            //            changeState(states.Get("hitStunAirborne"));
            //        }
            //        else {
            //            changeState(states.Get("hitStunStand"));
            //        }
            //    }
            //}

            public override void BeHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void BeBlocked(bool forceCrouch) {
                throw new InvalidOperationException("AttackGrounded is somehow being told to block");
            }

            public override void BeGrabbed() {
                changeState(states.Get("grabbed"));
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
                        FightingGameInputCodeDir direction = FightingGameInputCodeDir.Neutral;
                        fgChar.Use(combo);
                        fgChar.UseCombination<DirectionCurrent>(currDir => {
                            direction = fgChar.MapAbsoluteToRelative(((DirectionCurrent)currDir).direction);
                        });

                        fgChar.ChooseAttack(this, activeAttack, button, direction);
                        stop();
                    }
                }
            }
        }
    }
}