using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

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
                executeCallback = stateMachine.Enable(states.Get("fadeIn"));
                this.enabled = true;
            }

            public void LoadGame() {
                SceneManager.LoadScene(Persistence.Get().levelPath);
            }
        }
    }
}
