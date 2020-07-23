using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace DeviceManagement {
        public class GameDeviceMapping {
            public int displayNumber { set; get; }
            public InputDevice device { private set; get; }

            public GameDeviceMapping(InputDevice device) {
                this.device = device;
                displayNumber = -1;
            }

            public bool CompareDevice(InputDevice device) {
                return this.device == device;
            }
        }
    }
}