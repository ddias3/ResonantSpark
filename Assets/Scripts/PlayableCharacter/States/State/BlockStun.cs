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
        public abstract class BlockStun : CharacterBaseState {

            protected FightingGameInputCodeDir dirCurr;

            public new void Awake() {
                base.Awake();
            }

            protected void OnComplete() {
                //fgChar.BlockStunEnd();
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

            protected void OnDirectionCurrent(Action stop, Combination combo) {
                dirCurr = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }
        }
    }
}