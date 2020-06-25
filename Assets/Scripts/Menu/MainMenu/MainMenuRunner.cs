using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenuRunner : MenuRunner {

            public Menu mainMenu;

            public new void Start() {
                base.Start();

                this.enabled = false;
                StartCoroutine(DelayedStart());
            }

            public void Update() {
                executeCallback?.Invoke(0);
            }

            public IEnumerator DelayedStart() {
                yield return new WaitForEndOfFrame();
                (Action<int>, Action<int>) callbacks = stateMachine.Enable(states.Get("fadeIn"));
                executeCallback = callbacks.Item1;
                this.enabled = true;
            }
        }
    }
}
