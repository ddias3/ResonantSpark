using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class BlockStunCrouch : CharacterBaseState {

            private FightingGameInputCodeDir dirCurr;

            public new void Awake() {
                base.Awake();
                states.Register(this, "blockStunCrouch");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Block Stun", Color.white);

                fgChar.Play("idle_crouch");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHitBy(HitBox hitBox) {
                if (dirCurr == FightingGameInputCodeDir.Back) {
                    changeState(states.Get("blockStunStand"));
                }
                else if (dirCurr == FightingGameInputCodeDir.DownBack) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else {
                    changeState(states.Get("hitStunCrouch"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var dirPress = (DirectionCurrent) combo;
                dirCurr = fgChar.MapAbsoluteToRelative(dirPress.direction);
            }
        }
    }
}