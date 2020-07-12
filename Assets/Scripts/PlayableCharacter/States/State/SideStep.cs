using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;
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

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                throw new System.NotImplementedException();
            }

            public override void ExecutePass0(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public override void ExecutePass1(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public override void LateExecute(int frameIndex) {
                throw new NotImplementedException();
            }

            public override void Exit(int frameIndex) {
                throw new System.NotImplementedException();
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
                    throwable = true,
                };
            }

            public override void BeHit(bool launch) {
                changeState(states.Get("hitStunStand"));
            }

            public void OnDirectionPress(Action stop, Combination combo) {

            }

            public void OnDoubleTap(Action stop, Combination combo) {

            }

            public override void BeBlocked(bool forceCrouch) {
                throw new NotImplementedException();
            }

            public override void BeGrabbed() {
                throw new NotImplementedException();
            }

            public override bool CheckBlockSuccess(Hit hit) {
                throw new NotImplementedException();
            }
        }
    }
}