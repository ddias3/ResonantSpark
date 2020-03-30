using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class SideStep : CharacterBaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "sideStep");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                throw new System.NotImplementedException();
            }

            public override void Execute(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public override void Exit(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                throw new System.NotImplementedException();
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void GetHitBy(HitBox hitBox) {
                changeState(states.Get("hitStunStand"));
            }

            public void OnDirectionPress(Action stop, Combination combo) {

            }

            public void OnDoubleTap(Action stop, Combination combo) {

            }
        }
    }
}