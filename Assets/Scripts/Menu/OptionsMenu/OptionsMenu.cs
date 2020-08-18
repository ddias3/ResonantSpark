using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace Menu {
        public class OptionsMenu : Menu {
            public Animator animator2d;

            public ScrollSelect volumeSfx;
            public ScrollSelect volumeMusic;
            public Selectable retSelected;

            public Cursor2d cursor2d;

            private Selectable currSelected = null;

            public CloseOptionsEvent closeOptionsEvent;

            public override void Init() {
                if (currSelected == null) {
                    currSelected = volumeSfx;
                }

                List<ScrollSelectOption<string>> volumeSfxOptions = new List<ScrollSelectOption<string>>();
                for (int n = 0; n <= 10; ++n) {
                    volumeSfxOptions.Add(new ScrollSelectOption<string> { name = n.ToString(), callbackData = n.ToString() });
                }
                List<ScrollSelectOption<string>> volumeMusicOptions = new List<ScrollSelectOption<string>>();
                for (int n = 0; n <= 10; ++n) {
                    volumeMusicOptions.Add(new ScrollSelectOption<string> { name = n.ToString(), callbackData = n.ToString() });
                }
                volumeSfx.options = volumeSfxOptions;
                volumeMusic.options = volumeMusicOptions;

                int sfxIndex = (int)(Persistence.Get().GetOptionValue("sfxVolume") * 10);
                int musicIndex = (int)(Persistence.Get().GetOptionValue("musicVolume") * 10);
                volumeSfx.SetInitSelected(sfxIndex);
                volumeMusic.SetInitSelected(musicIndex);

                cursor2d.Hide();
                animator2d.Play("hidden");

                eventHandler.On("activate", () => {
                    Debug.Log("Options Menu Activate");
                    animator2d.Play("appear", 0, 0.0f);

                    cursor2d.Highlight(currSelected);
                });
                eventHandler.On("deactivate", () => {
                    animator2d.Play("disappear", 0, 0.0f);

                    cursor2d.Fade();
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
                    if (currSelected == retSelected) {
                        cursor2d.Select(retSelected, () => {
                            closeOptionsEvent.Invoke();
                        });
                    }
                });
                eventHandler.On("cancel", () => {
                    cursor2d.Highlight(retSelected);
                    currSelected = retSelected;
                });

                retSelected.On("up", () => {
                    cursor2d.Highlight(volumeMusic);
                    currSelected = volumeMusic;
                });

                volumeMusic.On("down", () => {
                    cursor2d.Highlight(retSelected);
                    currSelected = retSelected;
                }).On("up", () => {
                    cursor2d.Highlight(volumeSfx);
                    currSelected = volumeSfx;
                });

                volumeSfx.On("down", () => {
                    cursor2d.Highlight(volumeMusic);
                    currSelected = volumeMusic;
                });
            }

            public void AddListenerOnClose(UnityAction callback) {
                closeOptionsEvent.AddListener(callback);
            }
        }
    }
}