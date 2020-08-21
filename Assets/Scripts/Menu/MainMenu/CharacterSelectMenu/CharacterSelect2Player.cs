using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterSelect2Player : Selectable {
            public GameObject characterSelectIconPrefab;
            public Cursor3d cursor3dP1;
            public Cursor3d cursor3dP2;

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

            private int currHighlightedP1;
            private int currHighlightedP2;

            private string selectedP1;
            private string selectedP2;

            private GameDeviceMapping devMapP1;
            private GameDeviceMapping devMapP2;

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
                    for (int m = 0; m < charInfos[n].previews.Count; ++m) {
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

                eventHandler.On("activate", () => {
                    currHighlightedP1 = 0;
                    currHighlightedP2 = 0;
                    selectedP1 = "char";
                    selectedP2 = "char";

                    devMapP1 = persistence.devices[0];
                    devMapP2 = persistence.devices[1];

                    gameObject.SetActive(true);
                    colorSelectP1Text.gameObject.SetActive(false);
                    colorSelectP2Text.gameObject.SetActive(false);
                    readyP1.gameObject.SetActive(false);
                    readyP2.gameObject.SetActive(false);
                    cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                    cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("activate");
                    }
                });
                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                    cursor3dP1.Fade();
                    cursor3dP2.Fade();
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
                    if (devMap == devMapP1) {
                        if (selectedP1 == "char") {
                            currHighlightedP1 -= 1;
                            if (currHighlightedP1 < 0) {
                                currHighlightedP1 = charInfos.Count - 1;
                            }
                            cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                        }
                        else if (selectedP1 == "color") {
                            colorSelectP1.TriggerEvent("left", devMap);
                        }
                    }
                    else if (devMap == devMapP2) {
                        if (selectedP2 == "char") {
                            currHighlightedP2 -= 1;
                            if (currHighlightedP2 < 0) {
                                currHighlightedP2 = charInfos.Count - 1;
                            }
                            cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                        }
                        else if (selectedP2 == "color") {
                            colorSelectP2.TriggerEvent("left", devMap);
                        }
                    }
                });

                On("right", (GameDeviceMapping devMap) => {
                    if (devMap == devMapP1) {
                        if (selectedP1 == "char") {
                            currHighlightedP1 += 1;
                            if (currHighlightedP1 >= charInfos.Count) {
                                currHighlightedP1 = 0;
                            }
                            cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                        }
                        else if (selectedP1 == "color") {
                            colorSelectP1.TriggerEvent("right", devMap);
                        }
                    }
                    else if (devMap == devMapP2) {
                        if (selectedP2 == "char") {
                            currHighlightedP2 += 1;
                            if (currHighlightedP2 >= charInfos.Count) {
                                currHighlightedP2 = 0;
                            }
                            cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                        }
                        else if (selectedP2 == "color") {
                            colorSelectP2.TriggerEvent("right", devMap);
                        }
                    }
                });

                On("submit", (GameDeviceMapping devMap) => {
                    if (devMapP1 == devMap || devMapP2 == devMap) {
                        if (selectedP1 == "complete" && selectedP2 == "complete") {
                            // TODO: Play animation signaling.
                            onMenuCompleteCallback();
                        }
                        else {
                            if (devMap == devMapP1) {
                                if (selectedP1 == "char") {
                                    cursor3dP1.Select(selectableIcons[currHighlightedP1]);
                                    charSelectMenu.UpdateBillboard(1, charInfos[currHighlightedP1].previews[colorSelectP1.GetCurrentSelected()], charInfos[currHighlightedP1].name);
                                    persistence.SetCharacterSelected(0, "lawrence");
                                    colorSelectP1Text.gameObject.SetActive(true);
                                    cursor3dP1.Fade();
                                    selectedP1 = "color";
                                }
                                else if (selectedP1 == "color") {
                                    charSelectMenu.UpdateBillboard(1, charInfos[currHighlightedP1].previews[colorSelectP1.GetCurrentSelected()], charInfos[currHighlightedP1].name);
                                    persistence.SetColorSelected(0, colorSelectP1.GetCurrentSelected());
                                    readyP1.gameObject.SetActive(true);
                                    selectedP1 = "complete";
                                }
                            }
                            else if (devMap == devMapP2) {
                                if (selectedP2 == "char") {
                                    cursor3dP2.Select(selectableIcons[currHighlightedP2]);
                                    charSelectMenu.UpdateBillboard(2, charInfos[currHighlightedP2].previews[colorSelectP2.GetCurrentSelected()], charInfos[currHighlightedP2].name);
                                    persistence.SetCharacterSelected(1, "lawrence");
                                    colorSelectP2Text.gameObject.SetActive(true);
                                    cursor3dP2.Fade();
                                    selectedP2 = "color";
                                }
                                else if (selectedP2 == "color") {
                                    charSelectMenu.UpdateBillboard(2, charInfos[currHighlightedP2].previews[colorSelectP2.GetCurrentSelected()], charInfos[currHighlightedP2].name);
                                    persistence.SetColorSelected(1, colorSelectP2.GetCurrentSelected());
                                    readyP2.gameObject.SetActive(true);
                                    selectedP2 = "complete";
                                }
                            }
                        }
                    }
                });

                On("cancel", (GameDeviceMapping devMap) => {
                    if (devMapP1 == devMap || devMapP2 == devMap) {
                        if (selectedP1 == "char" || selectedP2 == "char") {
                            // TODO: Play animation signaling.
                            onMenuCancelCallback();
                        }
                        else {
                            if (devMap == devMapP1) {
                                if (selectedP1 == "complete") {
                                    cursor3dP1.Select(selectableIcons[currHighlightedP1]);
                                    readyP1.gameObject.SetActive(false);
                                    selectedP1 = "color";
                                }
                                else if (selectedP1 == "color") {
                                    selectedP1 = "char";
                                    colorSelectP1Text.gameObject.SetActive(false);
                                    cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                                    charSelectMenu.ResetBillboard(1);
                                }
                            }
                            else if (devMap == devMapP2) {
                                if (selectedP2 == "complete") {
                                    cursor3dP2.Select(selectableIcons[currHighlightedP2]);
                                    readyP2.gameObject.SetActive(false);
                                    selectedP2 = "color";
                                }
                                else if (selectedP2 == "color") {
                                    selectedP2 = "char";
                                    colorSelectP2Text.gameObject.SetActive(false);
                                    cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                                    charSelectMenu.ResetBillboard(2);
                                }
                            }
                        }
                    }
                });
            }

            public void FadeCursor(int player) {
                if (player == 1) {
                    cursor3dP1.Fade();
                }
                else if (player == 2) {
                    cursor3dP2.Fade();
                }
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