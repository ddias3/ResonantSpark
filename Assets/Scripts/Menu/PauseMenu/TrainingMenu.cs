using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class TrainingMenu : Menu {
            public Animator animator2d;

            public Selectable dummyBlock;
            public Selectable retSelected;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public void Start() {
                if (currSelected == null) {
                    currSelected = dummyBlock;
                }

                cursor2d.Hide();
                animator2d.Play("hidden");

                eventHandler.On("activate", () => {
                    Debug.Log("Pause Menu Activate");
                    animator2d.Play("appear", 0, 0.0f);

                    cursor2d.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    animator2d.Play("disappear", 0, 0.0f);

                    cursor2d.Fade();
                });

                eventHandler.On("pause", () => {
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
                    if (currSelected == dummyBlock) {
                        cursor2d.Select(dummyBlock, () => {
                            Debug.Log("dummyBlock");
                        });
                    }
                    else if (currSelected == retSelected) {
                        cursor2d.Select(retSelected, () => {
                            changeState("pauseMenuTraining");
                        });
                    }
                });
                eventHandler.On("cancel", () => {
                    cursor2d.Highlight(retSelected);
                    currSelected = retSelected;
                });

                retSelected.On("up", () => {
                    cursor2d.Highlight(dummyBlock);
                    currSelected = dummyBlock;
                });

                dummyBlock.On("down", () => {
                    cursor2d.Highlight(retSelected);
                    currSelected = retSelected;
                });
            }
        }
    }
}