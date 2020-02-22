using UnityEngine;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Init : MonoBehaviour {

            public BaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                stateMachine.Enable(initState, frame);
            }
        }
    }
}