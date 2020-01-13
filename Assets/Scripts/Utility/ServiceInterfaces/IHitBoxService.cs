using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public interface IHitBoxService {
            HitBox DefaultPrefab();
            void HitBox(Vector3 position, int durationFrames);  // TODO: Associate this hitbox with the correct player
        }
    }
}
