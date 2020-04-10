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
        public class HitStunCrouch : HitStun {

            public int hitStunLength = 10;

            public new void Awake() {
                base.Awake();
                states.Register(this, "hitStunCrouch");

                tracker = new Utility.Tracker(hitStunLength, new Action(OnComplete));
            }

            public override void Enter(int frameIndex, IState previousState) {
                fgChar.__debugSetStateText("Hit Stun", Color.magenta);

                tracker.Track();

                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) {
                    fgChar.Play("hurt_crouch_0");
                }
                else {
                    fgChar.Play("hurt_crouch_1");
                }
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (tracker.frameCount > hitStunLength) {
                    changeState(states.Get("crouch"));
                }

                fgChar.CalculateFinalVelocity();
                tracker.Increment();
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
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }
        }
    }
}