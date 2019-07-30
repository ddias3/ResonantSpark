using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class BaseState : MonoBehaviour, State {

            protected Controller ctrl;

            public void Start() {
                ctrl = GetComponentInParent<Controller>();
            }

            public abstract void Enter(State previousState);
            public abstract void Execute(Action<State> changeState);
            public abstract void Exit();

            public abstract void ServeInput(FightingGameInputCodeDir direction);
            public abstract void ServeInput(in List<Input.Combinations.Combination> inputCombinations);
        }
    }
}
