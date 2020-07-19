using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class IntroSkip : MenuBaseState {
            public Animator camera;
            public Menu.MainMenu mainMenu;

            public float introTime = 0.4f;
            private float startTime = 0.0f;

            public new void Start() {
                base.Start();
                states.Register(this, "introSkip");
            }

            public override void TriggerEvent(string eventName) {
                // do nothing
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                camera.Play("introSkip");

                startTime = Time.time;
            }

            public override void Execute(int frameIndex) {
                if (Time.time - startTime > introTime) {
                    Debug.Log("Change State to Main Menu");
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