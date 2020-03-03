using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace Input {
        public class UpdateInputMenuScenes : MonoBehaviour {
            public void FixedUpdate() {
                InputSystem.Update();
            }
        }
    }
}