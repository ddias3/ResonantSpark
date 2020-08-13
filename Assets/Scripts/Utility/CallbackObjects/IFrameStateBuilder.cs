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
                    Action<Hit> armorCallback,
                    Action<TargetFG> trackCallback,
                    AudioClip soundClip,
                    Action<AudioResource> soundCallback,
                    Projectile projectile,
                    Action<Projectile> projectileCallback,
                    Action executeCallback);
        }
    }

    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {
            public List<int> hitCallbackIds { get; private set; }

            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public bool cancellableOnWhiff { get; private set; }
            public bool counterHit { get; private set; }

            public Action<Hit> armorCallback { get; private set; }

            public Action<TargetFG> trackCallback { get; private set; }

            public AudioClip soundClip { get; private set; }
            public Action<AudioResource> soundCallback { get; private set; }

            public Projectile projectile { get; private set; }
            public Action<Projectile> projectileCallback { get; private set; }

            public Action executeCallback { get; private set; }

            public IFrameStateBuilder SupplyAllInfo(
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff,
                    bool counterHit,
                    List<int> hitCallbackIds,
                    Action<Hit> armorCallback,
                    Action<TargetFG> trackCallback,
                    AudioClip soundClip,
                    Action<AudioResource> soundCallback,
                    Projectile projectile,
                    Action<Projectile> projectileCallback,
                    Action executeCallback) {
                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.cancellableOnWhiff = cancellableOnWhiff;
                this.counterHit = counterHit;

                this.hitCallbackIds.AddRange(hitCallbackIds);

                if (armorCallback != null) this.armorCallback = armorCallback;
                if (trackCallback != null) this.trackCallback = trackCallback;
                if (soundClip != null) this.soundClip = soundClip;
                if (soundCallback != null) this.soundCallback = soundCallback;
                if (projectile != null) this.projectile = projectile;
                if (projectileCallback != null) this.projectileCallback = projectileCallback;
                if (executeCallback != null) this.executeCallback = executeCallback;
                return this;
            }
        }
    }
}