using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MenuEventHandler {
            private List<Action<int>> onDownCallbacks;
            private List<Action<int>> onUpCallbacks;
            private List<Action<int>> onLeftCallbacks;
            private List<Action<int>> onRightCallbacks;

            private List<Action<int>> onSubmitCallbacks;
            private List<Action<int>> onCancelCallbacks;

            private List<Action<int>> onActivateCallbacks;
            private List<Action<int>> onDeactivateCallbacks;

            public MenuEventHandler() {
                onDownCallbacks = new List<Action<int>>();
                onUpCallbacks = new List<Action<int>>();
                onLeftCallbacks = new List<Action<int>>();
                onRightCallbacks = new List<Action<int>>();

                onSubmitCallbacks = new List<Action<int>>();
                onCancelCallbacks = new List<Action<int>>();

                onActivateCallbacks = new List<Action<int>>();
                onDeactivateCallbacks = new List<Action<int>>();
            }

            public void On(string eventName, Action<int> callback) {
                switch (eventName) {
                    case "down":
                        onDownCallbacks.Add(callback);
                        break;
                    case "up":
                        onUpCallbacks.Add(callback);
                        break;
                    case "left":
                        onLeftCallbacks.Add(callback);
                        break;
                    case "right":
                        onRightCallbacks.Add(callback);
                        break;
                    case "submit":
                        onSubmitCallbacks.Add(callback);
                        break;
                    case "cancel":
                        onCancelCallbacks.Add(callback);
                        break;
                    case "activate":
                        onActivateCallbacks.Add(callback);
                        break;
                    case "deactivate":
                        onDeactivateCallbacks.Add(callback);
                        break;
                }
            }

            public void TriggerEvent(string eventName) {
                List<Action<int>> callbacks = null;

                switch (eventName) {
                    case "down":
                        callbacks = onDownCallbacks;
                        break;
                    case "up":
                        callbacks = onUpCallbacks;
                        break;
                    case "left":
                        callbacks = onLeftCallbacks;
                        break;
                    case "right":
                        callbacks = onRightCallbacks;
                        break;
                    case "submit":
                        callbacks = onSubmitCallbacks;
                        break;
                    case "cancel":
                        callbacks = onCancelCallbacks;
                        break;
                    case "activate":
                        callbacks = onActivateCallbacks;
                        break;
                    case "deactivate":
                        callbacks = onDeactivateCallbacks;
                        break;
                }

                if (callbacks != null) {
                    foreach (Action<int> action in callbacks) {
                        action.Invoke(0);
                    }
                }
            }
        }
    }
}