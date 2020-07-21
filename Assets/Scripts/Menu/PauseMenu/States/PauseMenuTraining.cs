using System.Collections;
using System.Collections.Generic;
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