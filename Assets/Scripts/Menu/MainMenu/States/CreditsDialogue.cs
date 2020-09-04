using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace MenuStates {
        public class CreditsDialogue : MenuBaseState {
            public Menu.Menu creditsDialogue;
            public Animator retSelectableAnimator;

            public new void Start() {
                base.Start();
                states.Register(this, "creditsDialogue");

                creditsDialogue.SetChangeStateCallback(stateName => {
                    changeState(states.Get(stateName));
                });
                creditsDialogue.SetMenuStack(menuStack);
            }

            public override void TriggerEvent(string eventName) {
                creditsDialogue.TriggerEvent(eventName);
            }

            public override void TriggerEvent(string eventName, GameDeviceMapping devMap) {
                creditsDialogue.TriggerEvent(eventName, devMap);
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                retSelectableAnimator.Play("appear");
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                retSelectableAnimator.Play("disappear");
            }
        }
    }
}