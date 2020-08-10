using System;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IHitService {
            void RegisterHit(Hit hit);
            Hit Create(Action<IHitCallbackObject> buildCallback);
        }
    }
}
