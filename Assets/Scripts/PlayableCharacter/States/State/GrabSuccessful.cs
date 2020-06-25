using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class GrabSuccessful : CharacterBaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "grabSuccessful");

                RegisterInputCallbacks()
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Grab Successful", Color.red);

                //if (messages.Count > 0) {
                //    Combination combo = messages.Dequeue();
                //    combo.inUse = false;
                //}

                fgChar.Play("idle");
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
                fgChar.CalculateFinalVelocity();
            }

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                    //TODO: This could be for an air grab
                return GroundRelation.GROUNDED;
            }

            public override void GetHit(bool launch) {
                // TODO: Make it clear that the attack is ignored by displaying a particle.
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