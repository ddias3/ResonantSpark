using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameStateBuilder {
            IFrameStateBuilder SupplyAllStaticInfo(bool chainCancellable, bool specialCancellable, int hitDamage, int blockDamage, float hitStun, float blockStun);
            IFrameStateBuilder HitBoxCallbackIds(List<int> callbackIds);
        }
    }

    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {
            public List<int> hitBoxCallbackIds { get; private set; }
            public bool activateHitBox { get; private set; }

            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }

            public IFrameStateBuilder HitBoxCallbackIds(List<int> hitBoxCallbackIds) {
                this.hitBoxCallbackIds = hitBoxCallbackIds;
                return this;
            }

            public IFrameStateBuilder SupplyAllStaticInfo(bool chainCancellable, bool specialCancellable, int hitDamage, int blockDamage, float hitStun, float blockStun) {
                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.hitDamage = hitDamage;
                this.blockDamage = blockDamage;
                this.hitStun = hitStun;
                this.blockStun = blockStun;
                return this;
            }
        }
    }
}