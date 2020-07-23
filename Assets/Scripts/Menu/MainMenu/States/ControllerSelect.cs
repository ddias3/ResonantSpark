using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class ControllerSelect : MenuBaseState {
            public Menu.ControllerSelectMenu controllerSelect;

            public new void Start() {
                base.Start();
                states.Register(this, "controllerSelect");

                controllerSelect.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                controllerSelect.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                controllerSelect.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                controllerSelect.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);

                controllerSelect.ShowReturnButton();
            }

            public override void Execute(int frameIndex) {
                // TODO: figure this one out.
            }

            public override void Exit(int frameIndex) {
                controllerSelect.HideReturnButton();
            }
        }
    }
}
