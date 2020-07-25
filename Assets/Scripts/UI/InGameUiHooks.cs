using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace UI {
        public class InGameUiHooks : MonoBehaviour, IHookExpose {

            private Dictionary<string, UnityEventBase> hooks;

            public void Awake() {
                hooks = new Dictionary<string, UnityEventBase> {
                    //{ "test", new UnityEvent() },
                };
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }
        }
    }
}