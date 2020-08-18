using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class PauseMenuTraining : Menu {
            public PauseMenuRunner runner;
            public Animator animator2d;

            public Selectable resume;
            public Selectable training;
            public Selectable options;
            public Selectable exit;

            public Cursor2d cursor2d;

            public OptionsMenuHooks optionsMenuHooks;

            private Selectable currSelected = null;

            public override void Init() {
                if (currSelected == null) {
                    currSelected = resume;
                }

                HookReceive hookReceive = new HookReceive(optionsMenuHooks.GetHooks());
                hookReceive.HookIn("closeOptions", new UnityEngine.Events.UnityAction(CloseOptions));

                cursor2d.Hide();
                animator2d.Play("hidden");

                eventHandler.On("activate", () => {
                    Debug.Log("Pause Menu Activate");
                    animator2d.Play("appear", 0, 0.0f);

                    cursor2d.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    animator2d.Play("disappear", 0, 0.0f);

                    cursor2d.Fade();
                });

                eventHandler.On("pause", () => {
                    ResumeGame();
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
                    if (currSelected == resume) {
                        cursor2d.Select(resume, () => {
                            ResumeGame();
                        });
                    }
                    else if (currSelected == training) {
                        cursor2d.Select(training, () => {
                            LoadTrainingMenu();
                        });
                    }
                    else if (currSelected == options) {
                        cursor2d.Select(options, () => {
                            LoadOptionsMenu();
                        });
                    }
                    else if (currSelected == exit) {
                        cursor2d.Select(exit, () => {
                            runner.LoadMainMenu();
                        });
                    }
                });
                eventHandler.On("cancel", () => {
                    cursor2d.Highlight(resume);
                    currSelected = resume;
                });

                resume.On("down", () => {
                    cursor2d.Highlight(training);
                    currSelected = training;
                });

                training.On("down", () => {
                    cursor2d.Highlight(options);
                    currSelected = options;
                }).On("up", () => {
                    cursor2d.Highlight(resume);
                    currSelected = resume;
                });

                options.On("down", () => {
                    cursor2d.Highlight(exit);
                    currSelected = exit;
                }).On("up", () => {
                    cursor2d.Highlight(training);
                    currSelected = training;
                });

                exit.On("up", () => {
                    cursor2d.Highlight(options);
                    currSelected = options;
                });
            }

            private void ResumeGame() {
                menuStack.Pop(this);
                changeState("inactive");
            }

            private void LoadTrainingMenu() {
                changeState("trainingMenu");
            }

            private void LoadOptionsMenu() {
                changeState("optionsMenu");
            }

            private void CloseOptions() {
                changeState("pauseMenuTraining");
            }
        }
    }
}