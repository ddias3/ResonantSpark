using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IHitBoxService {
            HitBoxComponent DefaultPrefab();
            Transform GetEmptyHoldTransform();
            void Active(HitBox hitBox);

            void RegisterHitBox(HitBox hitBox);
        }
    }
}
