using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MenuEventHandler {
            private List<Action> onDownCallbacks;
            private List<Action> onUpCallbacks;
            private List<Action> onLeftCallbacks;
            private List<Action> onRightCallbacks;

            private List<Action> onSubmitCallbacks;
            private List<Action> onCancelCallbacks;

            private List<Action> onPauseCallbacks;

            private List<Action> onActivateCallbacks;
            private List<Action> onDeactivateCallbacks;

            public MenuEventHandler() {
                onDownCallbacks = new List<Action>();
                onUpCallbacks = new List<Action>();
                onLeftCallbacks = new List<Action>();
                onRightCallbacks = new List<Action>();

                onSubmitCallbacks = new List<Action>();
                onCancelCallbacks = new List<Action>();

                onPauseCallbacks = new List<Action>();

                onActivateCallbacks = new List<Action>();
                onDeactivateCallbacks = new List<Action>();
            }

            public void On(string eventName, Action callback) {
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
                    case "pause":
                        onCancelCallbacks.Add(callback);
                        break;
                    case "activate":
                        onActivateCallbacks.Add(callback);
                        break;
                    case "deactivate":
                        onDeactivateCallbacks.Add(callback);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid menu event binding: " + eventName);
                }
            }

            public void TriggerEvent(string eventName) {
                List<Action> callbacks = null;

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
                    case "pause":
                        callbacks = onPauseCallbacks;
                        break;
                    case "activate":
                        callbacks = onActivateCallbacks;
                        break;
                    case "deactivate":
                        callbacks = onDeactivateCallbacks;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid menu event triggered: " + eventName);
                }

                if (callbacks != null) {
                    foreach (Action action in callbacks) {
                        action.Invoke();
                    }
                }
            }
        }
    }
}