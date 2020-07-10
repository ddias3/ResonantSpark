using System;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IHitBoxService {
            HitBox DefaultPrefab();
            Transform GetEmptyHoldTransform();
            void Active(HitBox hitBox);

            HitBox Create(Action<IHitBoxCallbackObject> buildCallback);
            void RegisterHitBox(HitBox hitBox);
        }
    }
}
