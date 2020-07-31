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
        public class Block : CharacterBaseState {

            private FightingGameInputCodeDir dirCurr;

            public new void Awake() {
                base.Awake();
                states.Register(this, "block");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Back Dash", Color.gray);

                fgChar.Play("idle");
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
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
                return GroundRelation.GROUNDED;
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = false,
                };
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
                if (forceCrouch) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else {
                    if (dirCurr == FightingGameInputCodeDir.DownBack) {
                        changeState(states.Get("blockStunCrouch"));
                    }
                    else if (dirCurr == FightingGameInputCodeDir.Back) {
                        changeState(states.Get("blockStunStand"));
                    }
                }
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("A character in block stun is being grabbed");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                if (dirCurr == FightingGameInputCodeDir.DownBack) {
                    return hit.validBlocks.Contains(Character.BlockType.LOW);
                }
                else if (dirCurr == FightingGameInputCodeDir.Back) {
                    return hit.validBlocks.Contains(Character.BlockType.HIGH);
                }
                else {
                    return false;
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var dirPress = (DirectionCurrent) combo;
                dirCurr = fgChar.MapAbsoluteToRelative(dirPress.direction);
            }
        }
    }
}