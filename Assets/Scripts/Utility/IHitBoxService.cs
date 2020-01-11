using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IHitBoxService {
            void HitBox(Vector3 position, int durationFrames);  // TODO: Associate this hitbox with the correct player
        }
    }
}
