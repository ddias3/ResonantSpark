using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public interface IHitBoxService {
            HitBox DefaultPrefab();
            Transform GetEmptyHoldTransform();
            void Active(HitBox hitBox);

            void RegisterHitBox(HitBox hitBox);
        }
    }
}
