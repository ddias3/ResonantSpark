using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class ConfirmSelections : MenuBaseState {
            public Menu.Menu confirmSelectionDialogue;

            public new void Start() {
                base.Start();
                states.Register(this, "confirmSelections");

                confirmSelectionDialogue.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                confirmSelectionDialogue.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                confirmSelectionDialogue.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                confirmSelectionDialogue.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
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