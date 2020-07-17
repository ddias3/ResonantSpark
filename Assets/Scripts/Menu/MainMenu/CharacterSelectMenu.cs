using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterSelectMenu : Menu {
            public Menu controllerButtonBind;
            public Menu levelSelectMenu;
            public Menu controllerSelectMenu;

            public Animator animator3d;
            public Animator animator2d;
            public Animator animatorRet;
            public Animator animatorPlayerDesignation;

            public Animator camera;

            public Cursor2d cursor2d;

            public MeshRenderer player1Billboard;
            public MeshRenderer player2Billboard;
            public Material defaultMaterial;

            public TMPro.TMP_Text player1Name;
            public TMPro.TMP_Text player2Name;

            public CharacterSelect characterSelect;
            public Selectable retSelectable;

            private Selectable currSelected = null;

            private int playerChoosing = 1;

            public void Start() {
                if (currSelected == null) {
                    currSelected = characterSelect;
                }

                cursor2d.Hide();

                eventHandler.On("activate", () => {
                    playerChoosing = 1;
                    player1Billboard.materials = new Material[] { defaultMaterial };
                    player2Billboard.materials = new Material[] { defaultMaterial };
                    player1Name.text = "";
                    player2Name.text = "";

                    animator3d.SetFloat("speed", 1.0f);
                    animator2d.SetFloat("speed", 1.0f);
                    animatorRet.SetFloat("speed", 1.0f);
                    animatorPlayerDesignation.SetFloat("speed", 1.0f);

                    animatorPlayerDesignation.SetBool("hiding", false);
                    animator3d.SetBool("hiding", false);
                    animator2d.SetBool("hiding", false);
                    animatorRet.SetBool("hiding", false);

                    animator3d.Play("appear", 0, 0.0f);
                    animator2d.Play("appear", 0, 0.0f);
                    animatorRet.Play("appear", 0, 0.0f);

                    cursor2d.Hide();
                    currSelected = characterSelect;
                    characterSelect.TriggerEvent("activate");
                });
                eventHandler.On("deactivate", () => {
                    animator3d.SetFloat("speed", -1.0f);
                    animator2d.SetFloat("speed", -1.0f);
                    animatorRet.SetFloat("speed", -1.0f);
                    animatorPlayerDesignation.SetFloat("speed", -1.0f);

                    animatorPlayerDesignation.SetBool("hiding", true);
                    animator3d.SetBool("hiding", true);
                    animator2d.SetBool("hiding", true);
                    animatorRet.SetBool("hiding", true);

                    animator3d.Play("disappear", 0, 1.0f);
                    animator2d.Play("disappear", 0, 1.0f);
                    animatorRet.Play("disappear", 0, 1.0f);

                    cursor2d.Fade();
                    characterSelect.TriggerEvent("deactivate");
                });

                eventHandler.On("submit", () => {
                    if (currSelected == retSelectable) {
                        cursor2d.Select(retSelectable, () => {
                            Debug.Log("cursor2d.Select");
                            GoToControllerSelect();
                        });
                    }
                    else {
                        currSelected.TriggerEvent("submit");
                    }
                });
                eventHandler.On("cancel", () => {
                    if (currSelected == retSelectable) {
                        GoToControllerSelect();
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
                        currSelected = characterSelect;
                    }
                    else {
                        currSelected.TriggerEvent("up");
                    }
                });
                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });

                characterSelect.On("down", () => {
                    characterSelect.FadeCursor();
                    cursor2d.Highlight(retSelectable);

                    currSelected = retSelectable;
                }).On("submit", () => {
                    if (playerChoosing == 1) {
                        player1Billboard.materials = new Material[] { characterSelect.GetPlayerPortrait() };
                        player1Name.text = characterSelect.GetPlayerName();
                        Persistence.Get().SetCharacterSelected(0, "lawrence");
                        playerChoosing = 2;
                    }
                    else if (playerChoosing == 2) {
                        player2Billboard.materials = new Material[] { characterSelect.GetPlayerPortrait() };
                        player2Name.text = characterSelect.GetPlayerName();
                        Persistence.Get().SetCharacterSelected(1, "lawrence");
                        GoToLevelSelect();
                    }
                }).On("cancel", () => {
                    characterSelect.FadeCursor();
                    cursor2d.Highlight(retSelectable);

                    currSelected = retSelectable;
                });
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

            private void GoToControllerSelect() {
                player1Billboard.materials = new Material[] { defaultMaterial };
                player2Billboard.materials = new Material[] { defaultMaterial };
                player1Name.text = "";
                player2Name.text = "";

                animatorPlayerDesignation.Play("characterSelectToControllerSelect", 0, 0.0f);
                menuStack.Pop(this);
                menuStack.AddMenu(controllerSelectMenu);
                camera.Play("characterSelectToControllerSelect");
                changeState("controllerSelect");
            }

            private void GoToLevelSelect() {
                animatorPlayerDesignation.Play("characterSelectToOutside", 0, 0.0f);
                menuStack.Pop(this);
                menuStack.AddMenu(levelSelectMenu);
                camera.Play("characterSelectToLevelSelect");
                changeState("levelSelect");
            }
        }
    }
}