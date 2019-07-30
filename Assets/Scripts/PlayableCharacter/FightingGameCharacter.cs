using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResonantSpark.CharacterStates;

namespace ResonantSpark {
    public class FightingGameCharacter : MonoBehaviour {

        private Controller stateMachineController;
        public StateMachine stateMachine;

        private GameObject target;
        private GameObject opponentChar;

        public BaseState idle;

        public GameObject opponentCharacter {
            set { opponentChar = value; }
        }

        public void Start() {
            //stateMachineController = GetComponent<Controller>();
            stateMachine.Enable(idle);
        }

        public void ServeInput(FightingGameInputCodeDir direction) {
            BaseState currState = (BaseState) stateMachine.GetCurrentState();
            currState.ServeInput(direction);
        }

        public void ServeInput(in List<Input.Combinations.Combination> inputCombinations) {
            BaseState currState = (BaseState) stateMachine.GetCurrentState();

            currState.ServeInput(in inputCombinations);
        }
    }
}