using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Idle : BaseState {

            public override void Enter(State previousState) {
                ctrl.Play("idle", 0, 0.0f);
            }

            public override void Execute(Action<State> changeState) {
                // TODO: Create callback
            }

            public override void Exit() {
                // do nothing
            }

            public override void ServeInput(FightingGameInputCodeDir direction) {
                throw new NotImplementedException();
            }
            public override void ServeInput(in List<Combination> inputCombinations) {
                    // TODO: Actual input use
                //inputCombinations.RemoveAll(x => true);
            }
        }
    }
}