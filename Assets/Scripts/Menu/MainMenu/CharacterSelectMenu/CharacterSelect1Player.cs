using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterSelect1Player : Selectable {
            public GameObject characterSelectIconPrefab;
            public Cursor3d cursor3d;

            public List<Character.CharacterInfo> charInfos;
            public List<Transform> charPortraitLocations;

            public ScrollSelect colorSelectP1;
            public ScrollSelect colorSelectP2;
            public TMPro.TMP_Text colorSelectP1Text;
            public TMPro.TMP_Text colorSelectP2Text;

            public TMPro.TMP_Text readyP1;
            public TMPro.TMP_Text readyP2;

            private CharacterSelectMenu charSelectMenu;

            private List<CharacterIconSelectable> selectableIcons;

            private int currSelected;
            private string playerSelecting;

            private int player1Selected;
            private int player2Selected;

            private Action onMenuCompleteCallback;
            private Action onMenuCancelCallback;

            public void Start() {
                selectableIcons = new List<CharacterIconSelectable>();
                for (int n = 0; n < charInfos.Count; ++n) {
                    CharacterIconSelectable charIcon = Instantiate(characterSelectIconPrefab,
                            charPortraitLocations[n].position,
                            Quaternion.identity,
                            transform)
                        .GetComponent<CharacterIconSelectable>();
                    charIcon.SetMaterial(charInfos[n].previews[0]);

                    selectableIcons.Add(charIcon);
                }
                for (int n = 0; n < charInfos.Count; ++n) {
                    var colorsP1 = new List<ScrollSelectOption<string>>();
                    var colorsP2 = new List<ScrollSelectOption<string>>();
                    for (int m = 0; m < charInfos[n].previews.Count - 1; ++m) {
                        colorsP1.Add(new ScrollSelectOption<string> {
                            name = "Color " + (m + 1),
                            callbackData = m.ToString(),
                        });
                        colorsP2.Add(new ScrollSelectOption<string> {
                            name = "Color " + (m + 1),
                            callbackData = m.ToString(),
                        });
                    }
                    colorSelectP1.options = colorsP1;
                    colorSelectP2.options = colorsP2;
                }

                Persistence persistence = Persistence.Get();

                currSelected = 0;

                eventHandler.On("activate", () => {
                    currSelected = 0;
                    playerSelecting = "char1";
                    gameObject.SetActive(true);
                    colorSelectP1Text.gameObject.SetActive(false);
                    colorSelectP2Text.gameObject.SetActive(false);
                    readyP1.gameObject.SetActive(false);
                    readyP2.gameObject.SetActive(false);
                    cursor3d.Highlight(selectableIcons[currSelected]);
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("activate");
                    }
                });
                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                    cursor3d.Fade();
                    colorSelectP1Text.gameObject.SetActive(false);
                    colorSelectP2Text.gameObject.SetActive(false);
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("deactivate");
                    }
                });

                On("up", (GameDeviceMapping devMap) => {
                    // These could be used for up and down, but there is only 1 character
                });

                On("down", (GameDeviceMapping devMap) => {
                    // These could be used for up and down, but there is only 1 character
                });

                On("left", (GameDeviceMapping devMap) => {
                    if (playerSelecting == "complete") return;

                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (playerSelecting == "char1" || playerSelecting == "char2") {
                            currSelected -= 1;
                            if (currSelected < 0) {
                                currSelected = charInfos.Count - 1;
                            }
                            cursor3d.Highlight(selectableIcons[currSelected]);
                        }
                        else if (playerSelecting == "color1") {
                            colorSelectP1.TriggerEvent("left", devMap);
                        }
                        else if (playerSelecting == "color2") {
                            colorSelectP2.TriggerEvent("left", devMap);
                        }
                    }
                });

                On("right", (GameDeviceMapping devMap) => {
                    if (playerSelecting == "complete") return;

                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (playerSelecting == "char1" || playerSelecting == "char2") {
                            currSelected += 1;
                            if (currSelected >= charInfos.Count) {
                                currSelected = 0;
                            }
                            cursor3d.Highlight(selectableIcons[currSelected]);
                        }
                        else if (playerSelecting == "color1") {
                            colorSelectP1.TriggerEvent("right", devMap);
                        }
                        else if (playerSelecting == "color2") {
                            colorSelectP2.TriggerEvent("right", devMap);
                        }
                    }
                });

                On("submit", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (playerSelecting == "char1") {
                            charSelectMenu.UpdateBillboard(1, charInfos[currSelected].previews[colorSelectP1.GetCurrentSelected()], charInfos[currSelected].name);
                            persistence.SetCharacterSelected(0, "lawrence");
                            colorSelectP1Text.gameObject.SetActive(true);
                            cursor3d.Fade();
                            playerSelecting = "color1";
                        }
                        else if (playerSelecting == "color1") {
                            charSelectMenu.UpdateBillboard(1, charInfos[currSelected].previews[colorSelectP1.GetCurrentSelected()], charInfos[currSelected].name);
                            persistence.SetColorSelected(0, colorSelectP1.GetCurrentSelected());
                            cursor3d.Highlight(selectableIcons[currSelected]);
                            readyP1.gameObject.SetActive(true);
                            playerSelecting = "char2";
                        }
                        else if (playerSelecting == "char2") {
                            charSelectMenu.UpdateBillboard(2, charInfos[currSelected].previews[colorSelectP2.GetCurrentSelected()], charInfos[currSelected].name);
                            persistence.SetCharacterSelected(1, "lawrence");
                            colorSelectP2Text.gameObject.SetActive(true);
                            cursor3d.Fade();
                            playerSelecting = "color2";
                        }
                        else if (playerSelecting == "color2") {
                            charSelectMenu.UpdateBillboard(2, charInfos[currSelected].previews[colorSelectP2.GetCurrentSelected()], charInfos[currSelected].name);
                            persistence.SetColorSelected(1, colorSelectP2.GetCurrentSelected());
                            readyP2.gameObject.SetActive(true);
                            playerSelecting = "complete";
                        }
                        else if (playerSelecting == "complete") {
                            // TODO: Play animation signaling.
                            onMenuCompleteCallback();
                        }
                    }
                });

                On("cancel", (GameDeviceMapping devMap) => {
                    if (persistence.IsAlreadySetDeviceMap(devMap)) {
                        if (playerSelecting == "complete") {
                            charSelectMenu.ResetBillboard(2);
                            readyP2.gameObject.SetActive(false);
                            playerSelecting = "color2";
                        }
                        else if (playerSelecting == "color2") {
                            charSelectMenu.ResetBillboard(2);
                            colorSelectP2Text.gameObject.SetActive(false);
                            cursor3d.Highlight(selectableIcons[currSelected]);
                            playerSelecting = "char2";
                        }
                        else if (playerSelecting == "char2") {
                            charSelectMenu.ResetBillboard(1);
                            cursor3d.Fade();
                            readyP1.gameObject.SetActive(false);
                            playerSelecting = "color1";
                        }
                        else if (playerSelecting == "color1") {
                            charSelectMenu.ResetBillboard(1);
                            colorSelectP1Text.gameObject.SetActive(false);
                            cursor3d.Highlight(selectableIcons[currSelected]);
                            playerSelecting = "char1";
                        }
                        else if (playerSelecting == "char1") {
                            // TODO: Play animation signaling.
                            onMenuCancelCallback();
                        }
                    }
                });
            }

            public void FadeCursor() {
                cursor3d.Fade();
            }

            public void SetCharacterSelectMenu(CharacterSelectMenu charSelectMenu) {
                this.charSelectMenu = charSelectMenu;
            }

            public void SetOnMenuCompleteCallback(Action callback) {
                this.onMenuCompleteCallback = callback;
            }

            public void SetOnMenuCancelCallback(Action callback) {
                this.onMenuCancelCallback = callback;
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.forward;
            }

            public override float Width() {
                return 2.0f;
            }
        }
    }
}