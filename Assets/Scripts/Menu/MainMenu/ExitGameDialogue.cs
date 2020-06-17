using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class ExitGameDialogue : Menu {
            public Animator animator;

            public Selectable cancel;
            public Selectable quit;

            public Cursor2d cursor;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = cancel;
                }

                cursor.Hide();

                eventHandler.On("activate", () => {
                    Debug.Log("Activate Exit Game Dialogue");
                    animator.SetFloat("speed", 1.0f);
                    animator.Play("appear", 0, 0.0f);

                    cursor.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    Debug.Log("Deactivate Exit Game Dialogue");
                    animator.SetFloat("speed", -1.0f);
                    animator.Play("appear", 0, 1.0f);

                    cursor.Fade();
                });

                eventHandler.On("left", () => {
                    currSelected.TriggerEvent("left");
                });
                eventHandler.On("right", () => {
                    currSelected.TriggerEvent("right");
                });

                eventHandler.On("submit", () => {
                    currSelected.TriggerEvent("submit");
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == cancel) {
                        menuStack.Pop(this);
                        changeState("mainMenu");
                    }
                    else {
                        cursor.Highlight(cancel);
                        currSelected = cancel;
                    }
                });

                cancel.On("right", () => {
                    cursor.Highlight(quit);
                    currSelected = quit;
                }).On("submit", () => {
                    cursor.Select(cancel);

                    menuStack.Pop(this);
                    changeState("mainMenu");
                });

                quit.On("left", () => {
                    cursor.Highlight(cancel);
                    currSelected = cancel;
                }).On("submit", () => {
                    cursor.Select(quit);

                    Debug.Log("Quit Application");
                    Application.Quit();
                });
            }
        }
    }
}