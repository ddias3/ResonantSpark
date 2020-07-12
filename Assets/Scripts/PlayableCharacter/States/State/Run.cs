using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Run : CharacterBaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "run");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    //.On<DoubleTap>(OnDoubleTap)
                    .On<NeutralReturn>(OnNeutralReturn);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Run", Color.black);
                //fgChar.Play("run_forward", 0, 0.0f);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
                fgChar.CalculateFinalVelocity();
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.RealignTarget();
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

            public override void BeHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void BeBlocked(bool forceCrouch) {
                throw new InvalidOperationException("A character is blocking while running");
            }

            public override void BeGrabbed() {
                throw new InvalidOperationException("A character in block stun is being grabbed");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }

            private void OnNeutralReturn(Action stop, Combination combo) {
                if (!combo.Stale(frame.index)) {
                    combo.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("idle"));
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress) combo;
                if (!dirPress.Stale(frame.index)) {
                    dirPress.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("walk"));
                }
            }

            //private void OnDoubleTap(Action stop, Combination combo) {
            //    var doubleTap = (DoubleTap) combo;
            //    if (!doubleTap.Stale(frame.index)) {
            //        doubleTap.inUse = true;
            //        stop.Invoke();
            //        changeState(states.Get("run").Message(combo));
            //    }
            //}
        }
    }
}