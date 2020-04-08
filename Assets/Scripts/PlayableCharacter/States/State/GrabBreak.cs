using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class GrabBreak : CharacterBaseState {

            public new void Awake() {
                base.Awake();

                // NOTE: this state might not be necessary.
                states.Register(this, "grabBreak");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Grab Break", new Color(1.0f, 0.5f, 0.0f));
                //if (messages.Count > 0) {
                //    Combination combo = messages.Dequeue();
                //    combo.inUse = false;
                //}

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
                // TODO: Make it clear that the attack is ignored by displaying a particle.

                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
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