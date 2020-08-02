using UnityEngine;

using ResonantSpark.Level;
using ResonantSpark.DeviceManagement;

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

            public override void Init() {
                if (currSelected == null) {
                    currSelected = levelSelect;
                }

                Persistence persistence = Persistence.Get();

                cursor2d.Hide();
                animator3d.Play("hidden");
                animatorRet.Play("hidden");
                levelSelect.TriggerEvent("deactivate");

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

                eventHandler_devMapping.On("left", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (currSelected != retSelectable) {
                            currSelected.TriggerEvent("left", devMap);
                        }
                    }
                });
                eventHandler_devMapping.On("right", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (currSelected != retSelectable) {
                            currSelected.TriggerEvent("right", devMap);
                        }
                    }
                });
                eventHandler_devMapping.On("up", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (currSelected == retSelectable) {
                            cursor2d.Fade();
                            currSelected = levelSelect;
                        }
                        else {
                            currSelected.TriggerEvent("up", devMap);
                        }
                    }
                });
                eventHandler_devMapping.On("down", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        currSelected.TriggerEvent("down", devMap);
                    }
                });
                eventHandler_devMapping.On("submit", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (currSelected == retSelectable) {
                            cursor2d.Select(retSelectable, () => {
                                GoToCharacterSelect();
                            });
                        }
                        else {
                            currSelected.TriggerEvent("submit", devMap);
                        }
                    }
                });
                eventHandler_devMapping.On("cancel", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (currSelected == retSelectable) {
                            GoToCharacterSelect();
                        }
                        else {
                            currSelected.TriggerEvent("cancel", devMap);
                        }
                    }
                });

                levelSelect.On("down", (GameDeviceMapping devMap) => {
                    //levelSelect.FadeCursor();
                    cursor2d.Highlight(retSelectable);

                    currSelected = retSelectable;
                }).On("submit", (GameDeviceMapping devMap) => {
                    LevelInfo levelInfo = levelSelect.GetSelection();
                    Persistence.Get().SetLevel(levelInfo.path);
                    GoToConfirm();
                }).On("cancel", (GameDeviceMapping devMap) => {
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
