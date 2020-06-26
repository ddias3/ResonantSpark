using System;

using UnityEngine;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class InitInGameEntityStateMachine : MonoBehaviour {
            public InGameEntityBaseState initState;
            public InGameEntityStateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                (Action<int>, Action<int>, Action<int>) executeCallbacks = stateMachine.Enable(initState);

                frame.AddUpdate((int) FramePriority.StateMachine, executeCallbacks.Item1);
                frame.AddUpdate((int) FramePriority.StateMachinePass1, executeCallbacks.Item2);
                frame.AddUpdate((int) FramePriority.LateStateMachine, executeCallbacks.Item3);
            }
        }
    }
}