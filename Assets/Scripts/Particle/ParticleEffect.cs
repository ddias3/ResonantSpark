using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Particle {
        public class ParticleEffect : MonoBehaviour, IResource {
            public List<ParticleSystem> subParticleSystems;

            private ParticleService service;
            private Vector3 deactivatedPosition;

            private bool inUse = false;

            public void Awake() {
                inUse = false;
            }

            public void SetUp(Vector3 position) {
                transform.position = position;
                foreach (ParticleSystem particle in subParticleSystems) {
                    particle.Play(true);
                }
            }

            public void SetService(ParticleService service) {
                this.service = service;
                deactivatedPosition = this.service.GetEmptyHoldTransform().position;
            }

            public bool IsActive() {
                bool atLeastOneActive = false;
                foreach (ParticleSystem particle in subParticleSystems) {
                    if (particle.IsAlive(true)) {
                        atLeastOneActive = true;
                    }
                }
                return atLeastOneActive;
            }

            public void Activate() {
                inUse = true;
            }

            public void Deactivate() {
                inUse = false;
                foreach (ParticleSystem particle in subParticleSystems) {
                    particle.Stop();
                }

                transform.position = deactivatedPosition;
            }
        }
    }
}