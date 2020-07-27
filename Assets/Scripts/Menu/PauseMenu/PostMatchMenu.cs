using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public enum PostMatchOption : int {
            None,
            Rematch = 1,
            LevelSelect = 2,
            CharacterSelect = 3,
            MainMenu = 4,
        }

        public class PostMatchSelect : UnityEvent<PostMatchOption> { }

        public class PostMatchMenu : Menu {
            public RectTransform p1Rect;
            public RectTransform p2Rect;

            public PostMatchSubmenu p1Menu;
            public PostMatchSubmenu p2Menu;

            private bool p1EnableMenu;
            private bool p2EnableMenu;

            private Vector3 p1MenuPos;
            private Vector3 p2MenuPos;

            private PostMatchOption p1Option;
            private PostMatchOption p2Option;

            public PostMatchSelect postMatchSelect = new PostMatchSelect();

            public override void Init() {
                p1EnableMenu = true;
                p2EnableMenu = true;
                p1MenuPos = p1Rect.localPosition;
                p2MenuPos = p2Rect.localPosition;

                p1Menu.AddOnSelectCallback(PlayerLabel.Player1, new UnityAction<PlayerLabel, PostMatchOption>(OnPostMatchSelect));
                p2Menu.AddOnSelectCallback(PlayerLabel.Player2, new UnityAction<PlayerLabel, PostMatchOption>(OnPostMatchSelect));
                p1Menu.AddOnDeselectCallback(PlayerLabel.Player1, new UnityAction<PlayerLabel>(OnPostMatchDeselect));
                p2Menu.AddOnDeselectCallback(PlayerLabel.Player2, new UnityAction<PlayerLabel>(OnPostMatchDeselect));

                eventHandler.On("activate", () => {
                    p1Option = PostMatchOption.None;
                    p2Option = PostMatchOption.None;

                    if (p1EnableMenu) p1Menu.TriggerEvent("activate");
                    if (p2EnableMenu) p2Menu.TriggerEvent("activate");
                });
                eventHandler.On("deactivate", () => {
                    if (p1EnableMenu) p1Menu.TriggerEvent("deactivate");
                    if (p2EnableMenu) p2Menu.TriggerEvent("deactivate");
                });

                eventHandler_devMapping.On("down", (GameDeviceMapping devMap) => {
                    if (p1EnableMenu && p1Option == PostMatchOption.None) {
                        p1Menu.TriggerEvent("down", devMap);
                    }
                    if (p2EnableMenu && p2Option == PostMatchOption.None) {
                        p2Menu.TriggerEvent("down", devMap);
                    }
                });
                eventHandler_devMapping.On("up", (GameDeviceMapping devMap) => {
                    if (p1EnableMenu && p1Option == PostMatchOption.None) {
                        p1Menu.TriggerEvent("up", devMap);
                    }
                    if (p2EnableMenu && p2Option == PostMatchOption.None) {
                        p2Menu.TriggerEvent("up", devMap);
                    }
                });
                eventHandler_devMapping.On("submit", (GameDeviceMapping devMap) => {
                    if (p1EnableMenu && p1Option == PostMatchOption.None) {
                        p1Menu.TriggerEvent("submit", devMap);
                    }
                    if (p2EnableMenu && p2Option == PostMatchOption.None) {
                        p2Menu.TriggerEvent("submit", devMap);
                    }
                });
                eventHandler_devMapping.On("cancel", (GameDeviceMapping devMap) => {
                    if (p1EnableMenu && p1Option != PostMatchOption.None) {
                        p1Menu.TriggerEvent("cancel", devMap);
                    }
                    if (p2EnableMenu && p2Option != PostMatchOption.None) {
                        p2Menu.TriggerEvent("cancel", devMap);
                    }
                });
            }

            public void SetDeviceMaps(GameDeviceMapping p1DevMap, GameDeviceMapping p2DevMap) {
                p1Menu.SetDeviceMap(p1DevMap);
                p2Menu.SetDeviceMap(p2DevMap);
            }

            public void SetDisplayBothMenus() {
                p1EnableMenu = true;
                p2EnableMenu = true;
                p1Rect.localPosition = p1MenuPos;
                p2Rect.localPosition = p2MenuPos;
            }

            public void SetDisplaySingleMenu(int playerId) {
                if (playerId == 1) {
                    p1EnableMenu = true;
                    p2EnableMenu = false;
                    p1Rect.localPosition = Vector3.zero;
                }
                else if (playerId == 2) {
                    p1EnableMenu = false;
                    p2EnableMenu = true;
                    p2Rect.localPosition = Vector3.zero;
                }
            }

            public void DisplayPostMatchMenu() {
                changeState("postMatchMenu");
            }

            private void OnPostMatchSelect(PlayerLabel playerLabel, PostMatchOption option) {
                if (playerLabel == PlayerLabel.Player1) {
                    p1Option = option;
                }
                else if (playerLabel == PlayerLabel.Player2) {
                    p2Option = option;
                }
                ChoosePostMatchOption();
            }

            private void OnPostMatchDeselect(PlayerLabel playerLabel) {
                if (playerLabel == PlayerLabel.Player1) {
                    p1Option = PostMatchOption.None;
                }
                else if (playerLabel == PlayerLabel.Player2) {
                    p2Option = PostMatchOption.None;
                }
            }

            private void ChoosePostMatchOption() {
                if (p1Option == PostMatchOption.MainMenu || p2Option == PostMatchOption.MainMenu) {
                    postMatchSelect.Invoke(PostMatchOption.MainMenu);
                }
                else if ((p1Option != PostMatchOption.None || !p1EnableMenu) && (p2Option != PostMatchOption.None || !p2EnableMenu)) {
                    if (p1Option > p2Option) {
                        postMatchSelect.Invoke(p1Option);
                    }
                    else {
                        postMatchSelect.Invoke(p2Option);
                    }
                }
            }
        }
    }
}