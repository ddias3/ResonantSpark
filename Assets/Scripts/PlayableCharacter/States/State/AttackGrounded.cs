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
            private FightingGameInputCodeBut button;
            private FightingGameInputCodeDir direction;

            private FightingGameInputCodeDir currDir;

            public new void Awake() {
                base.Awake();
                states.Register(this, "attackGrounded");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent)
                    .On<QuarterCircleButtonPress>(OnQuaterCircleButtonPress);
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

            public override void ReceiveHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("AttackGrounded is somehow being told to block");
            }

            public override void ReceiveGrabbed() {
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

                if (activeAttack.CancellableOnWhiff() && activeAttack.ChainCancellable()) {
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

                    // TODO: Create a mechanism for a frame 0 action, i.e. run the rest of the stand frame, then run the fist frame of the next action while pausing everything else.
                }
            }

            private void OnDirectionPlusButton(Action stop, Combination combo) {
                var dirPlusBut = (DirectionPlusButton)combo;

                CharacterProperties.Attack activeAttack = fgChar.GetCurrentAttack();

                if (activeAttack.CancellableOnWhiff() && activeAttack.ChainCancellable()) {
                    List<Combination> inputs = new List<Combination>();

                    fgChar.Use(combo);
                    inputs.Add(combo);

                    fgChar.ChooseAttack(this, null, inputs);
                    stop();

                    // TODO: Create a mechanism for a frame 0 action, i.e. run the rest of the stand frame, then run the fist frame of the next action while pausing everything else.
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