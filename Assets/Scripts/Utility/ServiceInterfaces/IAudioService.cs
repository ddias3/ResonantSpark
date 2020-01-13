using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IAudioService {
            AudioResource Use(Transform followTransform);

            // TODO: Interrupt audio resource
            // TODO: Stop using an audio resource
        }
    }
}