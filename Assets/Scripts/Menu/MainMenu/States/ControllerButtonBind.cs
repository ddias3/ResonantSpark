using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class ControllerButtonBind : MenuBaseState {
            public Menu.CharacterSelectMenu characterSelectMenu;
            public Menu.ControllerButtonBind buttonBind;

            public new void Start() {
                base.Start();
                states.Register(this, "controllerButtonBind");

                buttonBind.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
            }

            public override void TriggerEvent(string eventName) {
                characterSelectMenu.TriggerEvent(eventName);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(buttonBind);

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
