using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;

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

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public void OnDirectionPress(Action stop, Combination combo) {

            }

            public void OnDoubleTap(Action stop, Combination combo) {

            }
        }
    }
}