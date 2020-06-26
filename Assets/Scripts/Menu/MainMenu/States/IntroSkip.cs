using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class IntroSkip : MenuBaseState {
            public Animator camera;

            public float introTime = 0.4f;
            private float startTime = 0.0f;

            public new void Start() {
                base.Start();

                states.Register(this, "introSkip");

                // TODO: camera.onEvent("end intro"); or something like that
            }

            public override void TriggerEvent(string eventName) {
                // do nothing
            }

            public override void Enter(int frameIndex, IState previousState) {
                camera.Play("introSkip");

                startTime = Time.time;
            }

            public override void Execute(int frameIndex) {
                if (Time.time - startTime > introTime) {
                    changeState(states.Get("mainMenu"));
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}