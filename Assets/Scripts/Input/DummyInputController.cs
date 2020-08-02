using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer))]
        public class DummyInputController : InputController {
            public new void Awake() {
                base.Awake();

                inputBuffer.inputDelay = 0;
                inputBuffer.inputBufferSize = 30;
                inputBuffer.ResetBuffer();
            }

            public void SetInput(FightingGameAbsInputCodeDir direction) {
                inputBuffer.SetCurrentInputState(direction);
            }
        }
    }
}