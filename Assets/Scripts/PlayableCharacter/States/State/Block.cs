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
        public class Block : CharacterBaseState {

            private FightingGameInputCodeDir dirCurr;

            public new void Awake() {
                base.Awake();
                states.Register(this, "block");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Back Dash", Color.gray);

                fgChar.Play("idle");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
                fgChar.CalculateFinalVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHit(bool launch) {
                if (dirCurr == FightingGameInputCodeDir.Back) {
                    changeState(states.Get("blockStunStand"));
                }
                else if (dirCurr == FightingGameInputCodeDir.DownBack) {
                    changeState(states.Get("blockStunCrouch"));
                }
                else {
                    throw new InvalidOperationException("In Block State while pressing a direction other than back or down-back");
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var dirPress = (DirectionCurrent) combo;
                dirCurr = fgChar.MapAbsoluteToRelative(dirPress.direction);
            }
        }
    }
}