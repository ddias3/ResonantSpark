using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class VolumeChangeEvent : UnityEvent<float> { }
        public class CloseOptionsEvent : UnityEvent { }

        public class OptionsMenuHooks : MonoBehaviour, IHookExpose {

            public Transform stateMachineTransform;
            public RectTransform canvas;

            public OptionsMenu optionsMenu;

            public Transform optionsMenuTransform;
            public Transform optionsMenuStateTransform;

            private Dictionary<string, UnityEventBase> hooks;

            private VolumeChangeEvent volumeSfxChange;
            private VolumeChangeEvent volumeMusicChange;

            public void Awake() {
                optionsMenu.volumeSfx.AddListener(new UnityAction<string>(VolumeSfxChange));
                optionsMenu.volumeMusic.AddListener(new UnityAction<string>(VolumeMusicChange));

                optionsMenu.closeOptionsEvent = new CloseOptionsEvent();

                volumeSfxChange = new VolumeChangeEvent();
                volumeMusicChange = new VolumeChangeEvent();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "closeOptions", optionsMenu.closeOptionsEvent },
                    { "volumeSfxChange", volumeSfxChange },
                    { "volumeMusicChange", volumeMusicChange },
                };
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            public void SetUpOptionsMenu() {
                optionsMenuStateTransform.SetParent(stateMachineTransform, false);
                optionsMenuTransform.SetParent(canvas, false);
            }

            private void VolumeSfxChange(string data) {
                float volumeSfx = int.Parse(data) * 0.1f;
                Persistence.Get().SetOptionValue("sfxVolume", volumeSfx);
                volumeSfxChange.Invoke(volumeSfx);
            }

            private void VolumeMusicChange(string data) {
                float volumeMusic = int.Parse(data) * 0.1f;
                Persistence.Get().SetOptionValue("musicVolume", volumeMusic);
                volumeMusicChange.Invoke(volumeMusic);
            }
        }
    }
}