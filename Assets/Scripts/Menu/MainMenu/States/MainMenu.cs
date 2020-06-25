﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace MenuStates {
        public class MainMenu : MenuBaseState {
            public Menu.MainMenu mainMenu;

            public new void Start() {
                base.Start();
                states.Register(this, "mainMenu");

                mainMenu.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                mainMenu.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                mainMenu.TriggerEvent(eventName);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                mainMenu.ShowQuitButton();
            }

            public override void ExecutePass0(int frameIndex) {
                // TODO: figure this one out.
            }

            public override void ExecutePass1(int frameIndex) {
                throw new InvalidOperationException();
            }

            public override void Exit(int frameIndex) {
                mainMenu.HideQuitButton();
                mainMenu.DeactivateCursors();
            }
        }
    }
}