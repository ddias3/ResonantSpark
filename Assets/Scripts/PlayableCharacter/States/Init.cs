using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Init : MonoBehaviour {

            public BaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine() {
                stateMachine.Enable(initState);
            }
        }
    }
}