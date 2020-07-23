using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public abstract class Selectable : MonoBehaviour {
            protected MenuEventHandler eventHandler;
            protected MenuEventHandler<GameDeviceMapping> eventHandler_devMapping;

            public void Awake() {
                eventHandler = new MenuEventHandler();
                eventHandler_devMapping = new MenuEventHandler<GameDeviceMapping>();
            }

            public Selectable On(string eventName, Action callback) {
                eventHandler.On(eventName, callback);
                return this;
            }

            public Selectable On(string eventName, Action<GameDeviceMapping> callback) {
                eventHandler_devMapping.On(eventName, callback);
                return this;
            }

            public Selectable TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
                return this;
            }

            public void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                eventHandler_devMapping.TriggerEvent(eventName, devMap);
            }

            public abstract float Width();
            public abstract Vector3 Offset();
            public abstract Transform GetTransform();
        }
    }
}