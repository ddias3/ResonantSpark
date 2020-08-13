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
        public class GrabBreak : CharacterBaseState {

            public int grabBreakLength;

            private Utility.AttackTracker tracker;

            public new void Awake() {
                base.Awake();

                states.Register(this, "grabBreak");

                tracker = new Utility.AttackTracker();
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Grab Break", new Color(1.0f, 0.5f, 0.0f));

                fgChar.Play("grab_break");

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                //FindInput(fgChar.GetFoundCombinations());
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (tracker.frameCount > grabBreakLength) {
                    changeState(states.Get("stand"));
                }

                tracker.Increment();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override ComboState GetComboState() {
                return ComboState.InCombo;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = false,
                    throwable = false,
                };
            }

            public override void ReceiveHit(bool launch) {
                // TODO: Make it clear that the attack is ignored by displaying a particle.

                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                // TODO: something...
            }

            public override void ReceiveGrabbed() {
                // TODO: something...
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
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