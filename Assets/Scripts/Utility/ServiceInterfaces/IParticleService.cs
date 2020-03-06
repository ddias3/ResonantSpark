using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IParticleService {
            void PlayOneShot(Vector3 position, ParticleEffect partEffect);
        }
    }
}