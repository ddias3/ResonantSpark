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
        public abstract class AttackGrounded : Attack {

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

                RegisterEnterCallbacks()
                    .On<ButtonPress>(GivenButtonPress)
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                this.onCompleteAttack = new Action(OnCompleteAttack);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Attack", Color.red);
                // Start OnEnter with this
                GivenInput(fgChar.GetInUseCombinations());

                fgChar.ChooseAttack(this, queuedUpAttack, button, direction);

                activeAttack = queuedUpAttack;
                queuedUpAttack = null;

                activeAttack.StartPerformable(frameIndex);
                activeAttack.SetOnCompleteCallback(onCompleteAttack);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                activeAttack?.RunFrame();

                fgChar.CalculateFinalVelocity();
            }

            public override void Exit(int frameIndex) {
                activeAttack = null;
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHitBy(HitBox hitBox) {
                if (activeAttack != null) {
                    // TODO: Change this to be activeAttack.onHitStandState
                    if (activeAttack.groundRelation == GroundRelation.GROUNDED) {
                        changeState(states.Get("hitStunStand"));
                    }
                    else {
                        changeState(states.Get("hitStunCrouch"));
                    }
                    // else if (activeAttack.groundRelation == GroundRelation.GROUNDED) {
                    //      changeState(states.Get("hitStunAirborne"));
                    //}
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress)combo).button0;

                if (activeAttack == null || activeAttack.ChainCancellable()) {
                    // TODO: I need to change the input buffer to look further into the future than the input delay for a direction press.
                    fgChar.ChooseAttack(this, activeAttack, button, direction);
                    stop();
                }
            }

            private void GivenButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress)combo).button0;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                currDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }
        }
    }
}