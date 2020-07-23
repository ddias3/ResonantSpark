using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class PostMatchMenu : MenuBaseState {
            public Menu.PostMatchMenu postMatchMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "postMatchMenu");

                postMatchMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                postMatchMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                postMatchMenu.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                postMatchMenu.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(postMatchMenu);
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                menuStack.Pop(postMatchMenu);
            }
        }
    }
}