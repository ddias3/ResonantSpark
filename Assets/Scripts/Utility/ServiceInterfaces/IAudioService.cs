using UnityEngine;

using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace Service {
        public interface IAudioService {
            AudioResource Resource(Vector3 position, AudioClip audioClip, float playbackPosition = 0.0f);
            void SetOptionsMenuHook(OptionsMenuHooks optionsMenuHooks);
            void PlayOneShot(Vector3 position, AudioClip audioClip, float playbackPosition = 0.0f);
            void Play(AudioResource audioResource);
            void PlayMusic(string id);
        }
    }
}