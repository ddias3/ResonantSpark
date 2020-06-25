using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;

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

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Attack", Color.red);

                attackRunner.StartAttackIfRequired(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.RunAttackFrame();
                fgChar.CalculateFinalVelocity();
            }

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash, animatorVelocity);
            }

            public override void GetHit(bool launch) {
                CharacterProperties.Attack activeAttack = attackRunner.GetCurrentAttack();
                if (activeAttack.CounterHit()) {
                    if (launch) {
                        changeState(states.Get("hitStunAirborne"));
                    }
                    else {
                        changeState(states.Get("hitStunStand"));
                    }
                }
                else {
                    if (launch) {
                        changeState(states.Get("hitStunAirborne"));
                    }
                    else {
                        changeState(states.Get("hitStunStand"));
                    }
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress)combo).button0;

                CharacterProperties.Attack activeAttack = attackRunner.GetCurrentAttack();

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