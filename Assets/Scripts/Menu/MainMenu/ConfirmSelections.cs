using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class ConfirmSelections : Menu {
            public MainMenuRunner mainMenuRunner;

            public Animator animator;
            public Selectable confirm;
            public Cursor2d cursor;

            public void Start() {
                GameObject.FindGameObjectWithTag("rspMenu")
                    .GetComponent<MenuManager>().AddMenu(this);

                eventHandler.On("init", () => {
                    cursor.Hide();
                    animator.Play("hidden");
                });

                eventHandler.On("activate", () => {
                    animator.Play("appear", 0, 0.0f);

                    cursor.Highlight(confirm);
                });
                eventHandler.On("deactivate", () => {
                    animator.Play("disappear", 0, 0.0f);

                    cursor.Fade();
                });

                eventHandler.On("submit", () => {
                    cursor.Select(confirm, () => {
                        mainMenuRunner.LoadGame();
                    });
                });
                eventHandler.On("cancel", () => {
                    cursor.Fade();
                    menuStack.Pop(this);
                    changeState("levelSelect");
                });
            }
        }
    }
}