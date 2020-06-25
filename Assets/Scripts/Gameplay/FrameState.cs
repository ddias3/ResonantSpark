using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Character {
        public class FrameState {
            public List<Hit> hits { get; private set; }
            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public bool cancellableOnWhiff { get; private set; }
            public bool counterHit { get; private set; }

            public Action<HitInfo> armorCallback { get; private set; }

            public Action<Vector3, Transform> trackCallback { get; private set; }

            public AudioClip soundClip { get; private set; }
            public Action<AudioResource> soundCallback { get; private set; }

            public Projectile projectile { get; private set; }
            public Action<Projectile> projectileCallback { get; private set; }

            public FrameState(
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff,
                    bool counterHit,
                    List<Hit> hits,
                    Action<HitInfo> armorCallback,
                    Action<Vector3, Transform> trackCallback,
                    AudioClip soundClip,
                    Action<AudioResource> soundCallback,
                    Projectile projectile,
                    Action<Projectile> projectileCallback) {
                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.cancellableOnWhiff = cancellableOnWhiff;
                this.counterHit = counterHit;

                this.hits = hits;

                this.armorCallback = armorCallback;
                this.trackCallback = trackCallback;
                this.soundClip = soundClip;
                this.soundCallback = soundCallback;
                this.projectile = projectile;
                this.projectileCallback = projectileCallback;
            }

            public void Perform(FightingGameCharacter fgChar) {
                foreach (Hit hit in hits) {
                    hit.Active();
                }
                armorCallback?.Invoke(default);
                trackCallback?.Invoke(fgChar.GetTarget(), null);
                soundCallback?.Invoke(null);
                projectileCallback?.Invoke(projectile);
            }
        }
    }
}