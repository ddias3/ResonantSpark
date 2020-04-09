using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace Service {
        public class ParticleService : MonoBehaviour, IParticleService {
            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(ParticleService));
            }

            public void PlayOneShot(Vector3 position, ParticleEffect partEffect) {
                throw new System.NotImplementedException();
            }
        }
    }
}