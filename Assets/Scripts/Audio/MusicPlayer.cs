using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace Service {
        public class MusicPlayer : MonoBehaviour {
            public AudioSource musicPlayer;

            public List<string> ids;
            public List<AudioClip> clips;

            public void SetMusic(string id) {
                musicPlayer.clip = clips[ids.IndexOf(id)];
            }

            public void SetMusicVolume(float newVolume) {
                musicPlayer.volume = newVolume;
            }

            public void Play() {
                musicPlayer.Play();
            }
        }
    }
}