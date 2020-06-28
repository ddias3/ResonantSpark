using System;

using UnityEngine;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Utility {
        public class InitMultipassStateMachine : MonoBehaviour {
            public FramePriority callback0Priority;
            public bool performSecondPass;
            public FramePriority callback1Priority;
            public FramePriority lateCallbackPriority;

            public MultipassBaseState initState;
            public MultipassStateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                (Action<int>, Action<int>, Action<int>) executeCallbacks = stateMachine.Enable(initState);

                frame.AddUpdate((int) callback0Priority, executeCallbacks.Item1);
                if (performSecondPass) {
                    frame.AddUpdate((int) callback1Priority, executeCallbacks.Item2);
                }
                frame.AddUpdate((int) lateCallbackPriority, executeCallbacks.Item3);
            }
        }
    }
}