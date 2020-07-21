using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class Inactive : MenuBaseState {
            public PauseMenuRunner runner;
            public Menu.PauseMenuVersus pauseMenu;
            public Menu.PauseMenuTraining trainingMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "inactive");
            }

            public override void TriggerEvent(string eventName) {
                if (eventName == "pause") {
                    menuStack.AddMenu(trainingMenu);
                    changeState(states.Get("pauseMenuTraining"));
                }
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);

                runner.UnpauseGame();
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                runner.PauseGame();
            }
        }
    }
}