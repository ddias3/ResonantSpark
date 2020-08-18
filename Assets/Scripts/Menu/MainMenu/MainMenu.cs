using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenu : Menu {
            public Menu exitGameDialogue;
            public Menu controllerSelect;

            public Animator animator3d;
            public Animator animator2d;

            public Animator camera;

            public Selectable versus;
            public Selectable training;
            public Selectable options;
            public Selectable credits;

            public Selectable quit;

            public Selectable initSelectable;

            public Cursor3d cursor3d;
            public Cursor2d cursor2d;

            public OptionsMenuHooks optionsMenuHooks;

            private Selectable currSelected = null;

            public override void Init() {
                if (currSelected == null) {
                    currSelected = versus;
                }

                HookReceive hookReceive = new HookReceive(optionsMenuHooks.GetHooks());
                hookReceive.HookIn("closeOptions", new UnityEngine.Events.UnityAction(CloseOptions));

                cursor3d.Hide();
                cursor2d.Hide();
                animator3d.Play("hidden");
                animator2d.Play("hidden");

                eventHandler.On("activate", () => {
                    animator3d.SetFloat("speed", 1.0f);
                    animator2d.SetFloat("speed", 1.0f);
                    animator3d.SetBool("hiding", false);
                    animator2d.SetBool("hiding", false);
                    animator3d.Play("appear", 0, 0.0f);
                    animator2d.Play("appear", 0, 0.0f);

                    ActivateCursors();
                });
                eventHandler.On("deactivate", () => {
                    animator3d.SetFloat("speed", -1.0f);
                    animator2d.SetFloat("speed", -1.0f);
                    animator3d.SetBool("hiding", true);
                    animator2d.SetBool("hiding", true);
                    animator3d.Play("appear", 0, 1.0f);
                    animator2d.Play("appear", 0, 1.0f);

                    cursor3d.Fade();
                    cursor2d.Fade();
                });

                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });
                eventHandler.On("up", () => {
                    currSelected.TriggerEvent("up");
                });
                //eventHandler.On("left", () => {
                //    currSelected.TriggerEvent("left");
                //});
                //eventHandler.On("right", () => {
                //    currSelected.TriggerEvent("right");
                //});
                eventHandler.On("submit", () => {
                    if (currSelected == quit) {
                        cursor2d.Select(quit);

                        menuStack.AddMenu(exitGameDialogue);
                        changeState("exitGameDialogue");
                    }
                    else {
                        currSelected.TriggerEvent("submit");
                    }
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == quit) {
                        HideQuitButton();

                        menuStack.AddMenu(exitGameDialogue);
                        changeState("exitGameDialogue");
                    }
                    else {
                        cursor3d.Fade();
                        cursor2d.Highlight(quit);
                        currSelected = quit;
                    }
                });

                versus.On("down", () => {
                    cursor3d.Highlight(training);

                    currSelected = training;
                }).On("up", () => {
                    cursor3d.Fade();
                    cursor2d.Highlight(quit);

                    currSelected = quit;
                }).On("submit", () => {
                    Persistence.Get().SetGamemode("versus");

                    cursor3d.Select(versus, () => {
                        menuStack.Pop(this);
                        menuStack.AddMenu(controllerSelect);
                        camera.Play("mainMenuToControllerSelect");
                        changeState("controllerSelect");
                    });
                });

                training.On("down", () => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                }).On("up", () => {
                    cursor3d.Highlight(versus);

                    currSelected = versus;
                }).On("submit", () => {
                    Persistence.Get().SetGamemode("training");

                    cursor3d.Select(training, () => {
                        menuStack.Pop(this);
                        menuStack.AddMenu(controllerSelect);
                        camera.Play("mainMenuToControllerSelect");
                        changeState("controllerSelect");
                    });
                });

                options.On("down", () => {
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                }).On("up", () => {
                    cursor3d.Highlight(training);

                    currSelected = training;
                }).On("submit", () => {
                    cursor3d.Select(options, () => {
                        changeState("optionsMenu");
                    });
                });

                credits.On("down", () => {
                    cursor3d.Fade();
                    cursor2d.Highlight(quit);

                    currSelected = quit;
                }).On("up", () => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                }).On("submit", () => {
                    cursor3d.Select(credits, () => {
                        Debug.Log("Go To Credits Menu");
                    });
                });

                quit.On("down", () => {
                    cursor2d.Fade();
                    cursor3d.Highlight(versus);

                    currSelected = versus;
                }).On("up", () => {
                    cursor2d.Fade();
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                });
            }

            public void HideQuitButton() {
                animator2d.SetFloat("speed", -1.0f);
                animator2d.Play("appear", 0, 1.0f);
            }

            public void ShowQuitButton() {
                animator2d.SetFloat("speed", 5.0f);
                animator2d.Play("appear", 0, 0.0f);
                ActivateCursors();
            }

            public void DeactivateCursors() {
                cursor2d.Fade();
                cursor3d.Fade();
            }

            public void ActivateCursors() {
                if (currSelected == versus ||
                        currSelected == training ||
                        currSelected == options ||
                        currSelected == credits) {
                    cursor3d.Highlight(currSelected);
                }
                else {
                    cursor2d.Highlight(currSelected);
                }
            }

            private void CloseOptions() {
                changeState("mainMenu");
            }
        }
    }
}
