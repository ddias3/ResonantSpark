using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenu : Menu {
            public Selectable versus;
            public Selectable training;
            public Selectable options;
            public Selectable credits;

            public Selectable quit;

            public Selectable initSelectable;

            public Cursor3d cursor3d;
            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            private float transitionTime = 0.0f;
            private MenuState menuState;

            public new void Start() {
                base.Start();

                menuState = MenuState.Inactive;

                if (currSelected == null) {
                    currSelected = versus;
                }

                eventHandler.On("activate", x => {
                    animator.SetFloat("speed", 1.0f);
                    animator.Play("appear", 0, 0.0f);

                    cursor3d.Highlight(currSelected);
                    cursor2d.Fade();
                });
                eventHandler.On("deactivate", x => {
                    animator.SetFloat("speed", -1.0f);
                    animator.Play("appear", 0, 1.0f);
                });

                eventHandler.On("down", x => {
                    currSelected.TriggerEvent("down");
                });
                eventHandler.On("up", x => {
                    currSelected.TriggerEvent("up");
                });
                eventHandler.On("left", x => {
                    currSelected.TriggerEvent("left");
                });
                eventHandler.On("right", x => {
                    currSelected.TriggerEvent("right");
                });
                eventHandler.On("submit", x => {
                    currSelected.TriggerEvent("submit");
                });
                eventHandler.On("return", x => {
                    if (currSelected == quit) {
                        // TODO: bring up the Exit Game Dialogue.
                    }
                    else {
                        cursor3d.Fade();
                        cursor2d.Highlight(quit);
                        currSelected = quit;
                    }
                });

                versus.On("down", x => {
                    cursor3d.Highlight(training);

                    currSelected = training;
                }).On("up", x => {
                    cursor3d.Fade();
                    cursor2d.Highlight(quit);

                    currSelected = quit;
                }).On("submit", x => {

                });

                training.On("down", x => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                }).On("up", x => {
                    currSelected = versus;
                });

                options.On("down", x => {
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                }).On("up", x => {
                    cursor3d.Highlight(training);

                    currSelected = training;
                });

                credits.On("down", x => {
                    cursor3d.Fade();
                    cursor2d.Highlight(quit);

                    currSelected = quit;
                }).On("up", x => {
                    cursor3d.Highlight(options);

                    currSelected = options;
                });

                quit.On("down", x => {
                    cursor2d.Fade();
                    cursor3d.Highlight(versus);

                    currSelected = versus;
                }).On("up", x => {
                    cursor2d.Fade();
                    cursor3d.Highlight(credits);

                    currSelected = credits;
                });
            }

            public void Update() {
                switch (menuState) {
                    case MenuState.Inactive:
                        break;
                    case MenuState.Active:
                        break;
                }
            }
        }
    }
}