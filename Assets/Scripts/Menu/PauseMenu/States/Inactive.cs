using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Menu;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class Inactive : MenuBaseState {
            public PauseMenuRunner runner;
            public Menu.PauseMenuVersus pauseMenu;
            public Menu.PauseMenuTraining trainingMenu;

            public string pauseStateName;

            public new void Start() {
                base.Start();
                states.Register(this, "inactive");
            }

            public override void TriggerEvent(string eventName) {
                if (eventName == "pause") {
                    changeState(states.Get(pauseStateName));
                }
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                if (eventName == "pause") {
                    changeState(states.Get(pauseStateName));
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