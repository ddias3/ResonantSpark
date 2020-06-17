using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerSelect : Menu {
            public Menu controllerButtonBind;
            public Menu mainMenu;

            public Animator animator3d;
            public Animator animator2d;
            public Animator animatorPlayerDesignation;

            public Animator camera;

            public GameObject controllerIconPrefab;

            public Selectable controllerBinding;
            public Selectable retSelectable;

            public Selectable initSelectable;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = controllerBinding;
                }

                cursor2d.Hide();

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

                    currSelected = controllerBinding;
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

                    cursor2d.Fade();
                });

                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });
                eventHandler.On("up", () => {
                    currSelected.TriggerEvent("up");
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
                        changeState("mainMenu");
                    }
                    else {
                        cursor2d.Highlight(retSelectable);
                        currSelected = retSelectable;
                    }
                });

                retSelectable.On("up", () => {
                    cursor2d.Highlight(controllerBinding);

                    currSelected = controllerBinding;
                }).On("submit", () => {
                    menuStack.Pop(this);
                    menuStack.AddMenu(mainMenu);
                    camera.Play("controllerSelectToMainMenu");
                    changeState("mainMenu");
                }).On("cancel", () => {
                    menuStack.Pop(this);
                    menuStack.AddMenu(mainMenu);
                    camera.Play("controllerSelectToMainMenu");
                    changeState("mainMenu");
                });

                //TODO: Create the controller binding Selectable
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
