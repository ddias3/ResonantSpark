﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class ControllerSelect : MenuBaseState {
            public Animator camera;
            public Menu.MainMenu mainMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "controllerSelect");

                mainMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
            }

            public override void TriggerEvent(string eventName) {
                mainMenu.TriggerEvent(eventName);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                menuStack.AddMenu(mainMenu);

                mainMenu.ShowQuitButton();
                camera.Play("mainMenu");
            }

            public override void Execute(int frameIndex) {
                // TODO: figure this one out.
            }

            public override void Exit(int frameIndex) {
                mainMenu.HideQuitButton();
                mainMenu.DeactivateCursors();
            }
        }
    }
}
