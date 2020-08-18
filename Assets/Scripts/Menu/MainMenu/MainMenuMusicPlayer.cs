using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenuMusicPlayer : MonoBehaviour {
            public AudioSource musicPlayer;

            public OptionsMenuHooks optionsMenuHooks;

            public void Start() {
                musicPlayer.volume = Persistence.Get().GetOptionValue("musicVolume");
                HookReceive hookReceive = new HookReceive(optionsMenuHooks.GetHooks());
                hookReceive.HookIn<float>("volumeMusicChange", new UnityEngine.Events.UnityAction<float>(OnVolumeChange));
            }

            public void OnVolumeChange(float newVolume) {
                musicPlayer.volume = newVolume;
            }
        }
    }
}