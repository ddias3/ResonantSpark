using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameStateBuilder {
            IFrameStateBuilder SupplyAllStaticInfo(bool chainCancellable, bool specialCancellable, bool cancellableOnWhiff);
            IFrameStateBuilder HitCallbackIds(List<int> callbackIds);
        }
    }

    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {
            public List<int> hitCallbackIds { get; private set; }
            public bool activateHitBox { get; private set; }

            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public bool cancellableOnWhiff { get; private set; }

            public IFrameStateBuilder HitCallbackIds(List<int> hitCallbackIds) {
                this.hitCallbackIds = hitCallbackIds;
                return this;
            }

            public IFrameStateBuilder SupplyAllStaticInfo(
                    bool chainCancellable,
                    bool specialCancellable,
                    bool cancellableOnWhiff) {
                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.cancellableOnWhiff = cancellableOnWhiff;
                return this;
            }
        }
    }
}