using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public enum MenuState : int {
            Transition,
            Active,
            Inactive,
        }

        public abstract class Menu : MonoBehaviour {
            protected MenuEventHandler eventHandler;
            protected Action<string> changeState;
            protected MenuStack menuStack;

            public void Awake() {
                eventHandler = new MenuEventHandler();
            }

            public void TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
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
