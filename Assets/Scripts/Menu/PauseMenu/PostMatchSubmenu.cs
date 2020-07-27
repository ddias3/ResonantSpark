using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class PostMatchSubSelect : UnityEvent<PlayerLabel, PostMatchOption> { }
        public class PostMatchSubDeselect : UnityEvent<PlayerLabel> { }

        public class PostMatchSubmenu : Menu {
            public bool isPlayer1;
            public Animator animator2d;

            public Selectable rematch;
            public Selectable charSelect;
            public Selectable levelSelect;
            public Selectable mainMenu;

            public Cursor2d cursor2d;

            private GameDeviceMapping devMap;

            private Selectable currSelected = null;

            private PostMatchSubSelect postMatchSelect;
            private PostMatchSubDeselect postMatchDeselect;
            private PlayerLabel playerLabel;

            public new void Awake() {
                base.Awake();

                postMatchSelect = new PostMatchSubSelect();
                postMatchDeselect = new PostMatchSubDeselect();
            }

            public override void Init() {
                if (currSelected == null) {
                    currSelected = rematch;
                }

                cursor2d.Hide();
                animator2d.Play("hidden");

                eventHandler.On("activate", () => {
                    if (isPlayer1) {
                        animator2d.Play("appearP1", 0, 0.0f);
                    }
                    else {
                        animator2d.Play("appearP2", 0, 0.0f);
                    }

                    cursor2d.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    if (isPlayer1) {
                        animator2d.Play("disappearP1", 0, 0.0f);
                    }
                    else {
                        animator2d.Play("disappearP2", 0, 0.0f);
                    }

                    cursor2d.Fade();
                });

                eventHandler_devMapping.On("down", (GameDeviceMapping devMap) => {
                    if (this.devMap == devMap) {
                        currSelected.TriggerEvent("down");
                    }
                });
                eventHandler_devMapping.On("up", (GameDeviceMapping devMap) => {
                    if (this.devMap == devMap) {
                        currSelected.TriggerEvent("up");
                    }
                });
                eventHandler_devMapping.On("submit", (GameDeviceMapping devMap) => {
                    if (this.devMap == devMap) {
                        currSelected.TriggerEvent("submit");
                    }
                });
                eventHandler_devMapping.On("cancel", (GameDeviceMapping devMap) => {
                    if (this.devMap == devMap) {
                        cursor2d.Highlight(currSelected);
                        postMatchDeselect.Invoke(playerLabel);
                    }
                });

                rematch.On("up", () => {
                    cursor2d.Highlight(mainMenu);
                    currSelected = mainMenu;
                }).On("down", () => {
                    cursor2d.Highlight(mainMenu);//(charSelect);
                    currSelected = mainMenu;//charSelect;
                }).On("submit", () => {
                    cursor2d.Select(rematch, () => {
                        Debug.Log("Restart Selected");
                        cursor2d.DisplaySelected();
                        postMatchSelect.Invoke(playerLabel, PostMatchOption.Rematch);
                    });
                });

                //charSelect.On("up", () => {
                //    cursor2d.Highlight(rematch);
                //    currSelected = rematch;
                //}).On("down", () => {
                //    cursor2d.Highlight(levelSelect);
                //    currSelected = levelSelect;
                //}).On("submit", () => {
                //    cursor2d.Select(charSelect, () => {
                //        Debug.Log("Character Select Selected");
                //        postMatchSelect.Invoke(playerLabel, PostMatchOption.CharacterSelect);
                //    });
                //});

                //levelSelect.On("up", () => {
                //    cursor2d.Highlight(charSelect);
                //    currSelected = charSelect;
                //}).On("down", () => {
                //    cursor2d.Highlight(mainMenu);
                //    currSelected = mainMenu;
                //}).On("submit", () => {
                //    cursor2d.Select(levelSelect, () => {
                //        Debug.Log("Level Select Selected");
                //        postMatchSelect.Invoke(playerLabel, PostMatchOption.LevelSelect);
                //    });
                //});

                mainMenu.On("up", () => {
                    cursor2d.Highlight(rematch);//(levelSelect);
                    currSelected = rematch;//levelSelect;
                }).On("down", () => {
                    cursor2d.Highlight(rematch);
                    currSelected = rematch;
                }).On("submit", () => {
                    cursor2d.Select(mainMenu, () => {
                        Debug.Log("Main Menu Selected");
                        cursor2d.DisplaySelected();
                        postMatchSelect.Invoke(playerLabel, PostMatchOption.MainMenu);
                    });
                });
            }

            public void SetDeviceMap(GameDeviceMapping devMap) {
                this.devMap = devMap;
            }

            public void AddOnSelectCallback(PlayerLabel playerLabel, UnityAction<PlayerLabel, PostMatchOption> callback) {
                this.playerLabel = playerLabel;
                this.postMatchSelect.AddListener(callback);
            }

            public void AddOnDeselectCallback(PlayerLabel playerLabel, UnityAction<PlayerLabel> callback) {
                this.postMatchDeselect.AddListener(callback);
            }
        }
    }
}