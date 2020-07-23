using System.Collections;
using System.Collections.Generic;
using ResonantSpark.DeviceManagement;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class PauseMenuTraining : MenuBaseState {
            public Menu.PauseMenuTraining trainingMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "pauseMenuTraining");

                trainingMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                trainingMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                trainingMenu.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                trainingMenu.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(trainingMenu);
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}