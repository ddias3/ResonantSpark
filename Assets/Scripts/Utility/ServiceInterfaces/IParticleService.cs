using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace Service {
        public interface IParticleService {
            void PlayOneShot(Vector3 position, string effectId);
        }
    }
}