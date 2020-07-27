using UnityEngine;

using ResonantSpark.DeviceManagement;

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

            public CharacterSelect1Player characterSelect1P;
            public CharacterSelect2Player characterSelect2P;
            public Selectable retSelectable;

            private Selectable currSelected = null;

            private int playerChoosing = 1;

            public override void Init() {
                if (currSelected == null) {
                    currSelected = characterSelect1P;
                }

                characterSelect1P.SetCharacterSelectMenu(this);
                characterSelect2P.SetCharacterSelectMenu(this);

                characterSelect1P.SetOnMenuCompleteCallback(OnMenuComplete);
                characterSelect2P.SetOnMenuCompleteCallback(OnMenuComplete);

                characterSelect1P.SetOnMenuCancelCallback(OnMenuCancel);
                characterSelect2P.SetOnMenuCancelCallback(OnMenuCancel);

                Persistence persistence = Persistence.Get();

                cursor2d.Hide();
                characterSelect1P.TriggerEvent("deactivate");
                characterSelect2P.TriggerEvent("deactivate");

                eventHandler.On("activate", () => {
                    ResetBillboard(1);
                    ResetBillboard(2);

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
                    if (persistence.GetHumanPlayers() == 1) {
                        currSelected = characterSelect1P;
                    }
                    else if (persistence.GetHumanPlayers() == 2) {
                        currSelected = characterSelect2P;
                    }
                    currSelected.TriggerEvent("activate");
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
                    if (persistence.GetHumanPlayers() == 1) {
                        characterSelect1P.TriggerEvent("deactivate");
                    }
                    else if (persistence.GetHumanPlayers() == 2) {
                        characterSelect2P.TriggerEvent("deactivate");
                    }
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
                //eventHandler.On("cancel", () => {
                //    if (currSelected == retSelectable) {
                //        GoToControllerSelect();
                //    }
                //    else {
                //        cursor2d.Highlight(retSelectable);
                //        currSelected = retSelectable;
                //    }
                //});
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
                        //currSelected = characterSelect;
                    }
                    else {
                        currSelected.TriggerEvent("up");
                    }
                });
                eventHandler.On("down", () => {
                    currSelected.TriggerEvent("down");
                });

                eventHandler_devMapping.On("left", (GameDeviceMapping devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("left", devMap);
                    }
                });
                eventHandler_devMapping.On("right", (GameDeviceMapping devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("right", devMap);
                    }
                });
                eventHandler_devMapping.On("up", (GameDeviceMapping devMap) => {
                    if (currSelected == retSelectable) {
                        cursor2d.Fade();
                        //currSelected = characterSelect;
                    }
                    else {
                        currSelected.TriggerEvent("up", devMap);
                    }
                });
                eventHandler_devMapping.On("down", (GameDeviceMapping devMap) => {
                    currSelected.TriggerEvent("down", devMap);
                });
                eventHandler_devMapping.On("submit", (GameDeviceMapping devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("submit", devMap);
                    }
                });
                eventHandler_devMapping.On("cancel", (GameDeviceMapping devMap) => {
                    if (currSelected != retSelectable) {
                        currSelected.TriggerEvent("cancel", devMap);
                    }
                });

                //characterSelect1P.On("down", (GameDeviceMapping devMap) => {
                //    if (persistence.player1 == devMap || persistence.player2 == devMap) {
                //        characterSelect1P.FadeCursor();
                //        cursor2d.Highlight(retSelectable);
                //        currSelected = retSelectable;
                //    }
                //});

                //characterSelect2P.On("down", (GameDeviceMapping devMap) => {
                //    if (persistence.player1 == devMap) {
                //        characterSelect2P.FadeCursor(1);
                //    }
                //    else if (persistence.player2 == devMap) {
                //        characterSelect2P.FadeCursor(2);
                //    }
                //    cursor2d.Highlight(retSelectable);
                //});
            }

            public void OnMenuComplete() {
                GoToLevelSelect();
            }

            public void OnMenuCancel() {
                GoToControllerSelect();
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

            public void UpdateBillboard(int playerId, Material portrait, string name) {
                if (playerId == 1) {
                    player1Billboard.materials = new Material[] { portrait };
                    player1Name.text = name;
                }
                else if (playerId == 2) {
                    player2Billboard.materials = new Material[] { portrait };
                    player2Name.text = name;
                }
            }

            public void ResetBillboard(int playerId) {
                if (playerId == 1) {
                    player1Billboard.materials = new Material[] { defaultMaterial };
                    player1Name.text = "";
                }
                else if (playerId == 2) {
                    player2Billboard.materials = new Material[] { defaultMaterial };
                    player2Name.text = "";
                }
            }

            private void GoToControllerSelect() {
                ResetBillboard(1);
                ResetBillboard(2);

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