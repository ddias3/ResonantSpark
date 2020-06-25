using System;

using UnityEngine;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Init : MonoBehaviour {
            public FramePriority callback0Priority;
            public FramePriority callback1Priority;

            public bool enableCallback1;

            public BaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                (Action<int>, Action<int>) executeCallbacks = stateMachine.Enable(initState);

                frame.AddUpdate((int) callback0Priority, executeCallbacks.Item1);
                if (enableCallback1) {
                    frame.AddUpdate((int) callback1Priority, executeCallbacks.Item2);
                }
            }
        }
    }
}