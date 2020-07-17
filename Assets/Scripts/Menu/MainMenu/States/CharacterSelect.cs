using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class CharacterSelect : MenuBaseState {
            public Menu.CharacterSelectMenu characterSelectMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "characterSelect");

                characterSelectMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                characterSelectMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                characterSelectMenu.TriggerEvent(eventName);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                characterSelectMenu.ShowReturnButton();
            }

            public override void Execute(int frameIndex) {
                // TODO: figure this one out.
            }

            public override void Exit(int frameIndex) {
                characterSelectMenu.HideReturnButton();
            }
        }
    }
}
