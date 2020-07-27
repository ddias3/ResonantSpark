using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace Menu {
        public class PauseEvent : UnityEvent<bool> { }

        public class PauseMenuRunner : MenuRunner {
            public PauseEvent pauseEvent = new PauseEvent();
            public LoadMainMenu loadMainMenuEvent = new LoadMainMenu();

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

            public void ChangeState(string stateName) {
                stateMachine.QueueStateChange(states.Get(stateName));
            }

            public void LoadMainMenu() {
                //SceneManager.LoadScene("Scenes/Menu/MainMenu2");
                loadMainMenuEvent.Invoke();
            }

            public void PauseGame() {
                pauseEvent.Invoke(true);
            }

            public void UnpauseGame() {
                pauseEvent.Invoke(false);
            }
        }
    }
}