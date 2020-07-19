using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class PauseMenu : MenuBaseState {
            public Menu.PauseMenu pauseMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "pauseMenu");

                pauseMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                pauseMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                pauseMenu.TriggerEvent(eventName);
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