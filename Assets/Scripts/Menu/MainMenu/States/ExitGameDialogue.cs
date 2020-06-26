using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class ExitGameDialogue : MenuBaseState {
            public Menu.Menu exitGameDialogue;

            public new void Start() {
                base.Start();
                states.Register(this, "exitGameDialogue");

                exitGameDialogue.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                exitGameDialogue.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                exitGameDialogue.TriggerEvent(eventName);
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