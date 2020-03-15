using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Character {
        public class FrameState {
            public List<Hit> hits { get; private set; }
            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public bool cancellableOnWhiff { get; private set; }

            public Action<Vector3, Transform> trackCallback { get; private set; }

            public AudioClip soundClip { get; private set; }
            public Action<AudioResource> soundCallback { get; private set; }

            public Projectile projectile { get; private set; }
            public Action<Projectile> projectileCallback { get; private set; }

            public FrameState(
                    List<Hit> hits,
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff) {
                this.hits = hits;

                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.cancellableOnWhiff = cancellableOnWhiff;
            }

            public void Perform() {
                for (int n = 0; n < hits.Count; ++n) {
                    hits[n].Active();
                }

                trackCallback?.Invoke(default, null);
                soundCallback?.Invoke(null);
                projectileCallback?.Invoke(projectile);
            }
        }
    }
}