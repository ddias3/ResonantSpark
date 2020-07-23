using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerSelectMenu : Menu {
            public Menu controllerButtonBind;
            public Menu characterSelectMenu;
            public Menu mainMenu;

            public Animator animator3d;
            public Animator animator2d;
            public Animator animatorPlayerDesignation;

            public Animator camera;

            public ControllerSelect controllerSelect;
            public Selectable retSelectable;

            public Selectable initSelectable;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = initSelectable;
                }

                cursor2d.Hide();
                animator3d.Play("hidden");
                animator2d.Play("hidden");
                controllerSelect.TriggerEvent("deactivate");

                eventHandler.On("activate", () => {
                    animator3d.SetFloat("speed", 1.0f);
                    animator2d.SetFloat("speed", 1.0f);
                    animatorPlayerDesignation.SetFloat("speed", 1.0f);

                    animatorPlayerDesignation.SetBool("hiding", false);
                    animator3d.SetBool("hiding", false);
                    animator2d.SetBool("hiding", false);

                    animator3d.Play("appear", 0, 0.0f);
                    animator2d.Play("appear", 0, 0.0f);
                    animatorPlayerDesignation.Play("slideFromOutside", 0, 0.0f);

                    controllerSelect.TriggerEvent("activate");

                    currSelected = controllerSelect;
                    Debug.Log("cursor2d.Hide");
                    cursor2d.Hide();
                });
                eventHandler.On("deactivate", () => {
                    animator3d.SetFloat("speed", -1.0f);
                    animator2d.SetFloat("speed", -1.0f);
                    animatorPlayerDesignation.SetFloat("speed", -1.0f);

                    animatorPlayerDesignation.SetBool("hiding", true);
                    animator3d.SetBool("hiding", true);
                    animator2d.SetBool("hiding", true);

                    animator3d.Play("disappear", 0, 1.0f);
                    animator2d.Play("disappear", 0, 1.0f);
                    animatorPlayerDesignation.Play("slideFromOutside", 0, 1.0f);

                    controllerSelect.TriggerEvent("deactivate");
                    cursor2d.Fade();
                });

                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });
                eventHandler.On("up", () => {
                    if (currSelected == retSelectable) {
                        cursor2d.Fade();
                        currSelected = controllerSelect;
                    }
                    else {
                        currSelected.TriggerEvent("up");
                    }
                });
                eventHandler.On("submit", () => {
                    if (currSelected == retSelectable) {
                        cursor2d.Select(retSelectable, () => {
                            menuStack.Pop(this);
                            menuStack.AddMenu(mainMenu);
                            camera.Play("controllerSelectToMainMenu");
                            changeState("mainMenu");
                        });
                    }
                    else {
                        currSelected.TriggerEvent("submit");
                    }
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == retSelectable) {
                        menuStack.Pop(this);
                        menuStack.AddMenu(mainMenu);
                        camera.Play("controllerSelectToMainMenu");
                        changeState("mainMenu");
                    }
                    else {
                        currSelected.TriggerEvent("cancel");

                        //cursor2d.Highlight(retSelectable);
                        //currSelected = retSelectable;
                    }
                });

                eventHandler_devMapping.On("left", (devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("left", devMap);
                    }
                });
                eventHandler_devMapping.On("right", (devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("right", devMap);
                    }
                });

                eventHandler_devMapping.On("submit", (devMap) => {
                    if (currSelected == controllerSelect) {
                        currSelected.TriggerEvent("submit", devMap);
                    }
                });
                eventHandler_devMapping.On("cancel", (devMap) => {
                    if (currSelected == controllerSelect) {
                        currSelected.TriggerEvent("cancel", devMap);
                    }
                });

                controllerSelect.On("down", () => {
                    cursor2d.Highlight(retSelectable);
                    currSelected = retSelectable;
                });

                controllerSelect.OnMenuComplete((player1, player2) => {
                    Persistence persistence = Persistence.Get();
                    persistence.SetDeviceMappingP1(player1);
                    persistence.SetDeviceMappingP2(player2);

                    menuStack.Pop(this);
                    menuStack.AddMenu(characterSelectMenu);
                    animatorPlayerDesignation.Play("controllerSelectToCharacterSelect", 0, 0.0f);

                    camera.Play("controllerSelectToCharacterSelect");
                    changeState("characterSelect");
                });
            }

            public void HideReturnButton() {
                animator2d.SetBool("hiding", true);
                animator2d.SetFloat("speed", -1.0f);
                animator2d.Play("disappear", 0, 1.0f);
            }

            public void ShowReturnButton() {
                animator2d.SetBool("hiding", false);
                animator2d.SetFloat("speed", 1.0f);
                animator2d.Play("appear", 0, 0.0f);
            }
        }
    }
}
