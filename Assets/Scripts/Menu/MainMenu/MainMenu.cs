using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenu : Menu {
            public Animator animator3d;
            public Animator animator2d;

            public Selectable versus;
            public Selectable training;
            public Selectable options;
            public Selectable credits;

            public Selectable quit;

            public Selectable initSelectable;

            public Cursor3d cursor3d;
            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = versus;
                }

                cursor3d.Hide();
                cursor2d.Hide();

                eventHandler.On("activate", () => {
                    animator3d.SetFloat("speed", 1.0f);
                    animator2d.SetFloat("speed", 1.0f);
                    animator3d.Play("appear", 0, 0.0f);
                    animator2d.Play("appear", 0, 0.0f);

                    ActivateCursors();
                });
                eventHandler.On("deactivate", () => {
                    animator3d.SetFloat("speed", -1.0f);
                    animator2d.SetFloat("speed", -1.0f);
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
                    currSelected.TriggerEvent("submit");
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == quit) {
                        HideQuitButton();
                        changeState("mainMenu");
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
                    cursor3d.Select(versus);
                });

                training.On("down", () => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                }).On("up", () => {
                    cursor3d.Highlight(versus);

                    currSelected = versus;
                }).On("submit", () => {
                    cursor3d.Select(training);
                });

                options.On("down", () => {
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                }).On("up", () => {
                    cursor3d.Highlight(training);

                    currSelected = training;
                }).On("submit", () => {
                    cursor3d.Select(options);
                });

                credits.On("down", () => {
                    cursor3d.Fade();
                    cursor2d.Highlight(quit);

                    currSelected = quit;
                }).On("up", () => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                }).On("submit", () => {
                    cursor3d.Select(credits);
                });

                quit.On("down", () => {
                    cursor2d.Fade();
                    cursor3d.Highlight(versus);

                    currSelected = versus;
                }).On("up", () => {
                    cursor2d.Fade();
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                }).On("submit", () => {
                    cursor2d.Select(quit);

                    changeState("exitGameDialogue");
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
        }
    }
}
