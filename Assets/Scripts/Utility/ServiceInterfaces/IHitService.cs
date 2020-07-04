using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IHitService {
            void RegisterHit(Hit hit);
        }
    }
}
