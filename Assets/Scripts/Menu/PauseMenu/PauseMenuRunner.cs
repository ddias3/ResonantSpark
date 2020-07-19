using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ResonantSpark {
    namespace Menu {
        public class PauseMenuRunner : MenuRunner {

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
                executeCallback = stateMachine.Enable(states.Get("inactive"));
                this.enabled = true;
            }

            public void LoadMainMenu() {
                SceneManager.LoadScene("Scenes/Menu/MainMenu2");
            }
        }
    }
}