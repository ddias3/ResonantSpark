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
        public class HitStunStand : CharacterBaseState {

            private int testLength = 20;

            private Utility.AttackTracker tracker;

            public new void Awake() {
                base.Awake();
                states.Register(this, "hitStunStand");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);

                tracker = new Utility.AttackTracker(testLength);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Hit Stun", Color.magenta);

                tracker.Track(frameIndex);

                fgChar.Play("idle");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (tracker.frameCount > testLength) {
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

            public override void GetHitBy(HitBox hitBox) {
                changeState(states.Get("hitStunStand"));
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