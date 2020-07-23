using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public enum MenuState : int {
            Transition,
            Active,
            Inactive,
        }

        public abstract class Menu : MonoBehaviour {
            protected MenuEventHandler eventHandler;
            protected MenuEventHandler<GameDeviceMapping> eventHandler_devMapping;
            protected Action<string> changeState;
            protected MenuStack menuStack;

            public void Awake() {
                eventHandler = new MenuEventHandler();
                eventHandler_devMapping = new MenuEventHandler<GameDeviceMapping>();
            }

            public void TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
            }

            public void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                eventHandler_devMapping.TriggerEvent(eventName, devMap);
            }

            public void SetChangeStateCallback(Action<string> callback) {
                changeState = callback;
            }

            public void SetMenuStack(MenuStack menuStack) {
                this.menuStack = menuStack;
            }
        }
    }
}
