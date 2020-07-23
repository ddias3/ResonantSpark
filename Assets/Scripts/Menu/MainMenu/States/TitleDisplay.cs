using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using ResonantSpark.Utility;
using ResonantSpark.UI;
using ResonantSpark.Menu;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class TitleDisplay : MenuBaseState {
            public Animator titleDisplayPrompt;

            private InputAction anyKeyPress;

            public new void Start() {
                base.Start();

                states.Register(this, "titleDisplay");
            }

            private void AnyKeyPressed(InputAction.CallbackContext context) {
                changeState(states.Get("intro"));
            }

            public override void TriggerEvent(string eventName) {
                // do nothing
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                // do nothing
            }

            public override void Enter(int frameIndex, IState previousState) {
                anyKeyPress = new InputAction(binding: "/*/<button>");
                anyKeyPress.performed += AnyKeyPressed;
                anyKeyPress.Enable();

                inputManager.SetActiveState(this);
                titleDisplayPrompt.Play("display");
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                titleDisplayPrompt.Play("disappear");

                anyKeyPress.performed -= AnyKeyPressed;
                anyKeyPress.Disable();
                anyKeyPress.Dispose();
            }
        }
    }
}