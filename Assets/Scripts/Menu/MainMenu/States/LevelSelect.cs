using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class LevelSelect : MenuBaseState {
            public Menu.LevelSelectMenu levelSelectMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "levelSelect");

                levelSelectMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                levelSelectMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                levelSelectMenu.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                levelSelectMenu.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                levelSelectMenu.ShowReturnButton();
            }

            public override void Execute(int frameIndex) {
                // TODO: figure this one out.
            }

            public override void Exit(int frameIndex) {
                levelSelectMenu.HideReturnButton();
            }
        }
    }
}