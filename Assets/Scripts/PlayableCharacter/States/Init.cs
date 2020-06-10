using UnityEngine;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Init : MonoBehaviour {

            public BaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                frame.AddUpdate((int) FramePriority.StateMachine, stateMachine.Enable(initState));
            }
        }
    }
}