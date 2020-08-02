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

            private CharacterSelectMenu charSelectMenu;

            private List<CharacterIconSelectable> selectableIcons;

            private int currHighlightedP1;
            private int currHighlightedP2;

            private bool selectedP1;
            private bool selectedP2;

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
                    charIcon.SetMaterial(charInfos[n].preview);

                    selectableIcons.Add(charIcon);
                }

                Persistence persistence = Persistence.Get();

                eventHandler.On("activate", () => {
                    currHighlightedP1 = 1;
                    currHighlightedP2 = 1;
                    selectedP1 = false;
                    selectedP2 = false;

                    devMapP1 = persistence.devices[0];
                    devMapP2 = persistence.devices[1];

                    gameObject.SetActive(true);
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
                        if (!selectedP1) {
                            currHighlightedP1 -= 1;
                            if (currHighlightedP1 < 0) {
                                currHighlightedP1 = 2;
                            }
                            cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                        }
                    }
                    else if (devMap == devMapP2) {
                        if (!selectedP2) {
                            currHighlightedP2 -= 1;
                            if (currHighlightedP2 < 0) {
                                currHighlightedP2 = 2;
                            }
                            cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                        }
                    }
                });

                On("right", (GameDeviceMapping devMap) => {
                    if (devMap == devMapP1) {
                        if (!selectedP1) {
                            currHighlightedP1 += 1;
                            if (currHighlightedP1 > 2) {
                                currHighlightedP1 = 0;
                            }
                            cursor3dP1.Highlight(selectableIcons[currHighlightedP1]);
                        }
                    }
                    else if (devMap == devMapP2) {
                        if (!selectedP2) {
                            currHighlightedP2 += 1;
                            if (currHighlightedP2 > 2) {
                                currHighlightedP2 = 0;
                            }
                            cursor3dP2.Highlight(selectableIcons[currHighlightedP2]);
                        }
                    }
                });

                On("submit", (GameDeviceMapping devMap) => {
                    if (devMapP1 == devMap || devMapP2 == devMap) {
                        if (selectedP1 && selectedP2) {
                            // TODO: Play animation signaling.
                            onMenuCompleteCallback();
                        }
                        else {
                            if (devMap == devMapP1) {
                                selectedP1 = true;
                                cursor3dP1.Select(selectableIcons[currHighlightedP1]);
                                charSelectMenu.UpdateBillboard(1, charInfos[currHighlightedP1].preview, charInfos[currHighlightedP1].name);
                                persistence.SetCharacterSelected(0, "lawrence");
                            }
                            else if (devMap == devMapP2) {
                                selectedP2 = true;
                                cursor3dP2.Select(selectableIcons[currHighlightedP2]);
                                charSelectMenu.UpdateBillboard(2, charInfos[currHighlightedP2].preview, charInfos[currHighlightedP2].name);
                                persistence.SetCharacterSelected(1, "lawrence");
                            }
                        }
                    }
                });

                On("cancel", (GameDeviceMapping devMap) => {
                    if (devMapP1 == devMap || devMapP2 == devMap) {
                        if (!selectedP1 && !selectedP2) {
                            // TODO: Play animation signaling.
                            onMenuCancelCallback();
                        }
                        else {
                            if (devMap == devMapP1) {
                                selectedP1 = false;
                                charSelectMenu.ResetBillboard(1);
                            }
                            else if (devMap == devMapP2) {
                                selectedP2 = false;
                                charSelectMenu.ResetBillboard(2);
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