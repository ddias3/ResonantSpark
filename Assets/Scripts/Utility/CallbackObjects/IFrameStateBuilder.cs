using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameStateBuilder {
            IFrameStateBuilder SupplyAllInfo(
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff,
                    bool counterHit,
                    List<int> hitCallbackIds,
                    Action<HitInfo> armorCallback,
                    Action<Vector3, Transform> trackCallback,
                    AudioClip soundClip,
                    Action<AudioResource> soundCallback,
                    Projectile projectile,
                    Action<Projectile> projectileCallback);
        }
    }

    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {
            public List<int> hitCallbackIds { get; private set; }

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

            public IFrameStateBuilder SupplyAllInfo(
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff,
                    bool counterHit,
                    List<int> hitCallbackIds,
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

                this.hitCallbackIds = hitCallbackIds;

                if (armorCallback != null) this.armorCallback = armorCallback;
                if (trackCallback != null) this.trackCallback = trackCallback;
                if (soundClip != null) this.soundClip = soundClip;
                if (soundCallback != null) this.soundCallback = soundCallback;
                if (projectile != null) this.projectile = projectile;
                if (projectileCallback != null) this.projectileCallback = projectileCallback;
                return this;
            }
        }
    }
}