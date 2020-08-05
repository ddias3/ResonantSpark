using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class HitStunStand : HitStun {

            public new void Awake() {
                base.Awake();
                states.Register(this, "hitStunStand");
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Hit Stun", Color.magenta);

                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) {
                    fgChar.Play("hurt_stand_0");
                }
                else {
                    fgChar.Play("hurt_stand_1");
                }
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (fgChar.hitStun <= 0.0f) {
                    OnComplete();
                    changeState(states.Get("stand"));
                }
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                fgChar.IncrementHitStun();
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override void ReceiveHit(bool launch) {
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