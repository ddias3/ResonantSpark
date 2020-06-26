﻿using System;

using UnityEngine;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Utility {
        public class InitStateMachine : MonoBehaviour {
            public FramePriority callbackPriority;

            public BaseState initState;
            public StateMachine stateMachine;

            public void StartStateMachine(FrameEnforcer frame) {
                frame.AddUpdate((int) callbackPriority, stateMachine.Enable(initState));
            }
        }
    }
}
