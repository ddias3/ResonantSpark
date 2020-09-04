using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class CreditsDialogue : Menu {
            public Animator animator;

            public Selectable retSelectable;

            public Cursor2d cursor;

            private Selectable currSelected = null;

            public override void Init() {
                if (currSelected == null) {
                    currSelected = retSelectable;
                }

                cursor.Hide();
                animator.Play("hidden");

                eventHandler.On("activate", () => {
                    animator.Play("appear", 0, 0.0f);

                    cursor.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    animator.Play("disappear", 0, 0.0f);

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
                    if (currSelected == retSelectable) {
                        menuStack.Pop(this);
                        changeState("mainMenu");
                    }
                });

                retSelectable.On("submit", () => {
                    cursor.Select(retSelectable, () => {
                        menuStack.Pop(this);
                        changeState("mainMenu");
                    });
                });
            }
        }
    }
}