using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class Intro : MenuBaseState {
            public Animator camera;
            public Menu.MainMenu mainMenu;

            public float introTime = 4.0f;
            private float startTime = 0.0f;

            public new void Start() {
                base.Start();
                states.Register(this, "intro");
            }

            public override void TriggerEvent(string eventName) {
                if (eventName == "submit") {
                    changeState(states.Get("mainMenu"));
                }
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                camera.Play("intro");

                startTime = Time.time;
            }

            public override void Execute(int frameIndex) {
                if (Time.time - startTime > introTime) {
                    changeState(states.Get("mainMenu"));
                }
            }

            public override void Exit(int frameIndex) {
                camera.Play("mainMenu");
                menuStack.AddMenu(mainMenu);
            }
        }
    }
}