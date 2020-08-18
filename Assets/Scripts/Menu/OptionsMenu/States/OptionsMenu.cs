using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class OptionsMenu : MenuBaseState {
            public Menu.OptionsMenu optionsMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "optionsMenu");

                optionsMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                optionsMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                optionsMenu.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                optionsMenu.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(optionsMenu);
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                menuStack.Pop(optionsMenu);
            }
        }
    }
}
