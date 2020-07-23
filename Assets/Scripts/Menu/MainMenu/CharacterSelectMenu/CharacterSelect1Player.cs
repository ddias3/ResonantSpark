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

            private CharacterSelectMenu charSelectMenu;

            private List<CharacterIconSelectable> selectableIcons;

            private int currSelected;
            private int playerSelecting;

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
                    charIcon.SetMaterial(charInfos[n].preview);

                    selectableIcons.Add(charIcon);
                }

                Persistence persistence = Persistence.Get();

                eventHandler.On("activate", () => {
                    currSelected = 1;
                    playerSelecting = 1;
                    gameObject.SetActive(true);
                    cursor3d.Highlight(selectableIcons[currSelected]);
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("activate");
                    }
                });
                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                    cursor3d.Fade();
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
                    if (playerSelecting > 2) return;

                    if (persistence.player1 == devMap || persistence.player2 == devMap) {
                        currSelected -= 1;
                        if (currSelected < 0) {
                            currSelected = 2;
                        }
                        cursor3d.Highlight(selectableIcons[currSelected]);
                    }
                });

                On("right", (GameDeviceMapping devMap) => {
                    if (playerSelecting > 2) return;

                    if (persistence.player1 == devMap || persistence.player2 == devMap) {
                        currSelected += 1;
                        if (currSelected > 2) {
                            currSelected = 0;
                        }
                        cursor3d.Highlight(selectableIcons[currSelected]);
                    }
                });

                On("submit", (GameDeviceMapping devMap) => {
                    if (persistence.player1 == devMap || persistence.player2 == devMap) {
                        if (playerSelecting == 1) {
                            charSelectMenu.UpdateBillboard(1, charInfos[currSelected].preview, charInfos[currSelected].name);
                            persistence.SetCharacterSelected(0, "lawrence");
                            playerSelecting = 2;
                        }
                        else if (playerSelecting == 2) {
                            charSelectMenu.UpdateBillboard(2, charInfos[currSelected].preview, charInfos[currSelected].name);
                            persistence.SetCharacterSelected(1, "lawrence");
                            playerSelecting = 3;
                        }
                        else if (playerSelecting == 3) {
                            // TODO: Play animation signaling.
                            onMenuCompleteCallback();
                        }
                    }
                });

                On("cancel", (GameDeviceMapping devMap) => {
                    if (persistence.player1 == devMap || persistence.player2 == devMap) {
                        if (playerSelecting == 3) {
                            charSelectMenu.ResetBillboard(2);
                            playerSelecting = 2;
                        }
                        else if (playerSelecting == 2) {
                            charSelectMenu.ResetBillboard(1);
                            playerSelecting = 1;
                        }
                        else if (playerSelecting == 1) {
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