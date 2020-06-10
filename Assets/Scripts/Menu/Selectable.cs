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

            public Selectable On(string eventName, Action callback) {
                eventHandler.On(eventName, callback);
                return this;
            }

            public Selectable TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
                return this;
            }

            public abstract float Width();
            public abstract Vector3 Offset();
            public abstract Transform GetTransform();
        }
    }
}