using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.UI;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public class FadeIn : MenuBaseState {
            public Menu.MainMenu mainMenu;

            public Animator camera;
            public FadeInOut fadeIn;

            public float introTime = 0.4f;

            private float startTime = 0.0f;
            private bool playIntro = true;

            public new void Start() {
                base.Start();

                states.Register(this, "fadeIn");
                fadeIn.OnComplete(() => {
                    fadeIn.enabled = false;
                    fadeIn.gameObject.SetActive(false);
                });
            }

            public override void TriggerEvent(string eventName) {
                // do nothing
            }

            public override void Enter(int frameIndex, IState previousState) {
                inputManager.SetActiveState(this);
                startTime = Time.time;
                fadeIn.FadeIn();

                Persistence persistence = Persistence.Get();
                if (persistence.firstTimeLoad) {
                    persistence.MenuLoaded();
                    camera.Play("preintro");
                    playIntro = true;
                }
                else {
                    camera.Play("introSkip");
                    playIntro = false;
                }
            }

            public override void Execute(int frameIndex) {
                if (Time.time - startTime > introTime) {
                    camera.SetTrigger("start");

                    if (playIntro) {
                        changeState(states.Get("intro"));
                    }
                    else {
                        menuStack.AddMenu(mainMenu);
                        changeState(states.Get("mainMenu"));
                    }
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}