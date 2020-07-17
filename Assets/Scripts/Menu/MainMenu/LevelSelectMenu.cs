using UnityEngine;

using ResonantSpark.Level;

namespace ResonantSpark {
    namespace Menu {
        public class LevelSelectMenu : Menu {
            public Menu characterSelectMenu;
            public Menu confirmSelectionMenu;

            public Animator animator3d;
            public Animator animatorRet;
            public Animator animatorPlayerDesignation;

            public Animator camera;

            public LevelSelect levelSelect;
            public Selectable retSelectable;

            public Selectable initSelectable;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = levelSelect;
                }

                cursor2d.Hide();

                eventHandler.On("activate", () => {
                    Debug.Log("Activate");
                    animator3d.SetFloat("speed", 1.0f);
                    animatorRet.SetFloat("speed", 1.0f);
                    animatorPlayerDesignation.SetFloat("speed", 1.0f);

                    animatorPlayerDesignation.SetBool("hiding", false);
                    animator3d.SetBool("hiding", false);
                    animatorRet.SetBool("hiding", false);

                    animator3d.Play("appear", 0, 0.0f);
                    animatorRet.Play("appear", 0, 0.0f);

                    cursor2d.Hide();
                    currSelected = levelSelect;
                    levelSelect.TriggerEvent("activate");
                });
                eventHandler.On("deactivate", () => {
                    Debug.Log("Deactivate");
                    animator3d.SetFloat("speed", -1.0f);
                    animatorRet.SetFloat("speed", -1.0f);
                    animatorPlayerDesignation.SetFloat("speed", -1.0f);

                    animatorPlayerDesignation.SetBool("hiding", true);
                    animator3d.SetBool("hiding", true);
                    animatorRet.SetBool("hiding", true);

                    animator3d.Play("disappear", 0, 1.0f);
                    animatorRet.Play("disappear", 0, 1.0f);

                    cursor2d.Fade();
                    levelSelect.TriggerEvent("deactivate");
                });

                eventHandler.On("submit", () => {
                    if (currSelected == retSelectable) {
                        cursor2d.Select(retSelectable, () => {
                            Debug.Log("cursor2d.Select");
                            GoToCharacterSelect();
                        });
                    }
                    else {
                        currSelected.TriggerEvent("submit");
                    }
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == retSelectable) {
                        GoToCharacterSelect();
                    }
                    else {
                        cursor2d.Highlight(retSelectable);
                        currSelected = retSelectable;
                    }
                });
                eventHandler.On("left", () => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("left");
                    }
                });
                eventHandler.On("right", () => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("right");
                    }
                });
                eventHandler.On("up", () => {
                    if (currSelected == retSelectable) {
                        cursor2d.Fade();
                        currSelected = levelSelect;
                    }
                    else {
                        currSelected.TriggerEvent("up");
                    }
                });
                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });

                levelSelect.On("down", () => {
                    //levelSelect.FadeCursor();
                    cursor2d.Highlight(retSelectable);

                    currSelected = retSelectable;
                }).On("submit", () => {
                    LevelInfo levelInfo = levelSelect.GetSelection();
                    Persistence.Get().SetLevel(levelInfo.path);
                    GoToConfirm();
                }).On("cancel", () => {
                    //levelSelect.FadeCursor();
                    cursor2d.Highlight(retSelectable);

                    currSelected = retSelectable;
                });
            }

            private void GoToConfirm() {
                animatorPlayerDesignation.Play("characterSelectToOutside", 0, 0.0f);
                menuStack.AddMenu(confirmSelectionMenu);
                changeState("confirmSelections");
            }

            private void GoToCharacterSelect() {
                animatorPlayerDesignation.Play("controllerSelectToCharacterSelect", 0, 0.0f);
                menuStack.Pop(this);
                menuStack.AddMenu(characterSelectMenu);
                camera.Play("levelSelectToCharacterSelect");
                changeState("characterSelect");
            }

            public void HideReturnButton() {
                animatorRet.SetBool("hiding", true);
                animatorRet.SetFloat("speed", -1.0f);
                animatorRet.Play("disappear", 0, 1.0f);
            }

            public void ShowReturnButton() {
                animatorRet.SetBool("hiding", false);
                animatorRet.SetFloat("speed", 1.0f);
                animatorRet.Play("appear", 0, 0.0f);
            }
        }
    }
}
