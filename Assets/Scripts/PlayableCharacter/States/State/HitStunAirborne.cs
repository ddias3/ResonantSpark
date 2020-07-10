using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class HitStunAirborne : HitStun {

            public Vector3 gravityExtra;

            public new void Awake() {
                base.Awake();
                states.Register(this, "hitStunAirborne");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Hit Stun", Color.magenta);

                fgChar.Play("hurt_airborne");
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                fgChar.AddForce(gravityExtra, ForceMode.Acceleration);

                if (fgChar.hitStun <= 0.0f) {
                    OnComplete();
                    changeState(states.Get("recoverAirborne"));
                }
                if (fgChar.Grounded(out Vector3 landPoint)) {
                    OnComplete();
                    changeState(states.Get("land"));
                }

                //if (fgChar.CheckAboutToLand()) {
                //    changeState(states.Get("land"));
                //}
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                fgChar.IncrementHitStun();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.AIRBORNE;
            }

            public override void GetHit(bool launch) {
                changeState(states.Get("hitStunAirborne"));
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = (DirectionPress)combo;
                if (!dirPress.Stale(frame.index)) {
                    dirPress.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("walk"));//.Message(dirPress));
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap)combo;
                if (!doubleTap.Stale(frame.index)) {
                    doubleTap.inUse = true;
                    stop.Invoke();
                    changeState(states.Get("run"));//.Message(doubleTap));
                }
            }
        }
    }
}
