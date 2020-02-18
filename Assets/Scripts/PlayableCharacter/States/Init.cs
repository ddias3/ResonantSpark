using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Init : MonoBehaviour {

            public CharacterBaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                stateMachine.Enable(initState, frame);
            }
        }
    }
}