using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MenuEventHandler {
            private static readonly List<string> validEvents = new List<string> {
                "down",
                "up",
                "left",
                "right",

                "submit",
                "cancel",

                "pause",

                "activate",
                "deactivate",
            };
            private Dictionary<string, List<Action>> callbacks;

            public MenuEventHandler() {
                callbacks = new Dictionary<string, List<Action>>();
                for (int n = 0; n < validEvents.Count; ++n) {
                    callbacks.Add(validEvents[n], new List<Action>());
                }
            }

            public void On(string eventName, Action callback) {
                if (validEvents.Contains(eventName)) {
                    callbacks[eventName].Add(callback);
                }
                else {
                    throw new InvalidOperationException("Invalid menu event binding: " + eventName);
                }
            }

            public void TriggerEvent(string eventName) {
                List<Action> callbackList = null;

                if (validEvents.Contains(eventName)) {
                    callbackList = this.callbacks[eventName];
                }
                else {
                    throw new InvalidOperationException("Invalid menu event triggered: " + eventName);
                }

                if (callbackList != null) {
                    foreach (Action action in callbackList) {
                        action.Invoke();
                    }
                }
            }
        }
    }
}