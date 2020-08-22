using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Particle;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class ParticleService : MonoBehaviour, IParticleService {
            public ParticleEffect blockPrefab;
            public ParticleEffect hitPrefab;

            public Transform particleEmpty;
            private ResourceRecycler<ParticleEffect> blockEffects;
            private ResourceRecycler<ParticleEffect> hitEffects;

            private List<ParticleEffect> previousActiveEffects;
            private List<ParticleEffect> activeEffects;

            private List<ParticleEffect> activeOneShots;

            private FrameEnforcer frame;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.ActivePollingReset, new System.Action<int>(ResetActivePolling));

                blockEffects = new ResourceRecycler<ParticleEffect>(blockPrefab, Vector3.zero, 32, particleEmpty, resource => {
                    resource.SetService(this);
                    resource.Deactivate();
                });

                hitEffects = new ResourceRecycler<ParticleEffect>(hitPrefab, Vector3.zero, 32, particleEmpty, resource => {
                    resource.SetService(this);
                    resource.Deactivate();
                });

                previousActiveEffects = new List<ParticleEffect>();
                activeEffects = new List<ParticleEffect>();
                activeOneShots = new List<ParticleEffect>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(ParticleService));
            }

            public Transform GetEmptyHoldTransform() {
                return particleEmpty;
            }

            public void PlayOneShot(Vector3 position, string effectId) {
                ParticleEffect resource = null;
                if (effectId == "block") {
                    resource = blockEffects.UseResource();
                }
                else if (effectId == "hit") {
                    resource = hitEffects.UseResource();
                }

                resource.SetUp(position);

                activeOneShots.Add(resource);
            }

            private void FrameUpdate(int frameIndex) {
                foreach (ParticleEffect particle in activeEffects) {
                    if (!previousActiveEffects.Contains(particle)) {
                        if (!particle.IsActive()) {
                            particle.Activate();
                        }
                    }
                }

                foreach (ParticleEffect particle in previousActiveEffects) {
                    if (!activeEffects.Contains(particle)) {
                        particle.Deactivate();
                    }
                    else {
                        if (!particle.IsActive()) {
                            particle.Deactivate();
                        }
                    }
                }

                for (int n = 0; n < activeOneShots.Count; ++n) {
                    if (!activeOneShots[n].IsActive()) {
                        activeOneShots[n].Deactivate();
                        activeOneShots.RemoveAt(n);
                        --n;
                    }
                }
            }

            private void ResetActivePolling(int frameIndex) {
                previousActiveEffects.Clear();
                foreach (ParticleEffect particle in activeEffects) {
                    previousActiveEffects.Add(particle);
                }
                activeEffects.Clear();
            }
        }
    }
}