using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(FrameEnforcer))]
        public class UpdateInput : MonoBehaviour {
            private FrameEnforcer frame;

            public void Start() {
                frame = gameObject.GetComponent<FrameEnforcer>();
                frame.SetUpdate(new Action<int>(ProcessEventsManually));
            }

            public void ProcessEventsManually(int frameIndex) {
                InputSystem.Update();
            }
        }
    }
}