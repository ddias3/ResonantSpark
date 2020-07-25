using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class VersusHooks : MonoBehaviour, IHookExpose {

            public PauseMenuRunner pauseMenuRunner;

            private Dictionary<string, UnityEventBase> hooks;

            public void Awake() {
                hooks = new Dictionary<string, UnityEventBase> {
                    { "pause", pauseMenuRunner.pauseEvent },
                };
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }
        }
    }
}