using System;
using UnityEngine;

using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public class InputService : MonoBehaviour, IInputService {

            public HumanInputController player0;

            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(InputService));
            }

            public HumanInputController GetInputController(int controllerIndex) {
                return player0;
            }
        }
    }
}