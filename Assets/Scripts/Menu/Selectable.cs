using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public abstract class Selectable : MonoBehaviour {
            protected MenuEventHandler eventHandler;

            public void Awake() {
                eventHandler = new MenuEventHandler();
            }

            public Selectable On(string eventName, Action<int> callback) {
                eventHandler.On(eventName, callback);
                return this;
            }

            public Selectable TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
                return this;
            }
        }
    }
}