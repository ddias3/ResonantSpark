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
                    //.On<QuarterCircleButtonPress>(OnQuaterCircleButtonPress);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Attack", Color.red);

                fgChar.StartAttackIfRequired(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.RunAttackFrame();

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
                //fgChar.UpdateTarget(); fgChar.UpdateAttackTarget();
            }

            public override void Exit(int frameIndex) {
                fgChar.SetStandCollider(Vector3.zero);
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

                if (activeAttack.CancellableOnWhiff() && activeAttack.ChainCancellable()) {
                    if (button != FightingGameInputCodeBut.D) {
                        List<Combination> inputs = new List<Combination>();

                        fgChar.UseCombination<DirectionCurrent>(currDir => {
                            fgChar.Use(currDir);
                            inputs.Add(currDir);
                        });

                        fgChar.Use(combo);
                        inputs.Add(combo);

                        inputs.Sort((Combination a, Combination b) => {
                            return a.GetFrame() - b.GetFrame();
                        });

                        fgChar.ChooseAttack(this, activeAttack, inputs);
                        stop();
                    }
                }
            }

            private void OnQuaterCircleButtonPress(Action stop, Combination combo) {
                var quarterBut = (QuarterCircleButtonPress)combo;

                CharacterProperties.Attack activeAttack = fgChar.GetCurrentAttack();

                if (activeAttack.CancellableOnWhiff() && activeAttack.ChainCancellable()) {
                    List<Combination> inputs = new List<Combination>();
                    fgChar.Use(combo);
                    inputs.Add(combo);
                    fgChar.ChooseAttack(this, null, inputs);
                    stop();
                }
            }
        }
    }
}