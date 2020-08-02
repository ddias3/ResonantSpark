using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer))]
        public class DummyInputController : InputController {
            public void SetInput() {

            }
        }
    }
}