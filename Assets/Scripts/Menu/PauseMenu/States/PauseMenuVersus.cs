using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class PauseMenuVersus : MenuBaseState {
            public Menu.PauseMenuVersus pauseMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "pauseMenuVersus");

                pauseMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                pauseMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                pauseMenu.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                pauseMenu.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(pauseMenu);
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