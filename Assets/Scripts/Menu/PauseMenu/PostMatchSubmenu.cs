using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class PostMatchSubmenu : Menu {
            public bool isPlayer1;
            public Animator animator2d;

            public Selectable restart;
            public Selectable charSelect;
            public Selectable levelSelect;
            public Selectable mainMenu;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = restart;
                }

                GameObject.FindGameObjectWithTag("rspMenu")
                    .GetComponent<MenuManager>().AddMenu(this);

                eventHandler.On("init", () => {
                    cursor2d.Hide();
                    animator2d.Play("hidden");
                });
                eventHandler.On("activate", () => {
                    if (isPlayer1) {
                        animator2d.Play("appearP1", 0, 0.0f);
                    }
                    else {
                        animator2d.Play("appearP2", 0, 0.0f);
                    }

                    cursor2d.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    if (isPlayer1) {
                        animator2d.Play("disappearP1", 0, 0.0f);
                    }
                    else {
                        animator2d.Play("disappearP2", 0, 0.0f);
                    }

                    cursor2d.Fade();
                });

                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });
                eventHandler.On("up", () => {
                    currSelected.TriggerEvent("up");
                });
                eventHandler.On("submit", () => {
                    currSelected.TriggerEvent("submit");
                });
                eventHandler.On("cancel", () => {
                    // TODO: change color of selected to unselect.
                });

                restart.On("up", () => {
                    cursor2d.Highlight(mainMenu);
                    currSelected = mainMenu;
                }).On("down", () => {
                    cursor2d.Highlight(charSelect);
                    currSelected = charSelect;
                }).On("submit", () => {
                    cursor2d.Select(restart, () => {
                        Debug.Log("Restart Selected");
                    });
                });

                charSelect.On("up", () => {
                    cursor2d.Highlight(restart);
                    currSelected = restart;
                }).On("down", () => {
                    cursor2d.Highlight(levelSelect);
                    currSelected = levelSelect;
                }).On("submit", () => {
                    cursor2d.Select(charSelect, () => {
                        Debug.Log("Character Select Selected");
                    });
                });

                levelSelect.On("up", () => {
                    cursor2d.Highlight(charSelect);
                    currSelected = charSelect;
                }).On("down", () => {
                    cursor2d.Highlight(mainMenu);
                    currSelected = mainMenu;
                }).On("submit", () => {
                    cursor2d.Select(levelSelect, () => {
                        Debug.Log("Level Select Selected");
                    });
                });

                mainMenu.On("up", () => {
                    cursor2d.Highlight(levelSelect);
                    currSelected = levelSelect;
                }).On("down", () => {
                    cursor2d.Highlight(restart);
                    currSelected = restart;
                }).On("submit", () => {
                    cursor2d.Select(mainMenu, () => {
                        Debug.Log("Main Menu Selected");
                    });
                });
            }
        }
    }
}