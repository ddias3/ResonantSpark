using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace Utility {
        public class HookReceive {
            private Dictionary<string, UnityEventBase> hooks;

            public HookReceive(Dictionary<string, UnityEventBase> hooks) {
                this.hooks = hooks;
            }

            public void HookIn(string name, UnityAction callback) {
                foreach (KeyValuePair<string, UnityEventBase> hook in hooks.Where((kvp) => kvp.Key == name)) {
                    ((UnityEvent)hook.Value).AddListener(callback);
                }
            }

            public void HookIn<T0>(string name, UnityAction<T0> callback) {
                foreach (KeyValuePair<string, UnityEventBase> hook in hooks.Where((kvp) => kvp.Key == name)) {
                    ((UnityEvent<T0>)hook.Value).AddListener(callback);
                }
            }

            public void HookIn<T0, T1>(string name, UnityAction<T0, T1> callback) {
                foreach (KeyValuePair<string, UnityEventBase> hook in hooks.Where((kvp) => kvp.Key == name)) {
                    ((UnityEvent<T0, T1>)hook.Value).AddListener(callback);
                }
            }

            public void HookIn<T0, T1, T2>(string name, UnityAction<T0, T1, T2> callback) {
                foreach (KeyValuePair<string, UnityEventBase> hook in hooks.Where((kvp) => kvp.Key == name)) {
                    ((UnityEvent<T0, T1, T2>)hook.Value).AddListener(callback);
                }
            }

            public void HookIn<T0, T1, T2, T3>(string name, UnityAction<T0, T1, T2, T3> callback) {
                foreach (KeyValuePair<string, UnityEventBase> hook in hooks.Where((kvp) => kvp.Key == name)) {
                    ((UnityEvent<T0, T1, T2, T3>)hook.Value).AddListener(callback);
                }
            }
        }
    }
}