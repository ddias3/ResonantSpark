using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameStateBuilder {
            IFrameStateBuilder SupplyAllStaticInfo(bool chainCancellable, bool specialCancellable, int hitDamage, int blockDamage, float hitStun, float blockStun);
            IFrameStateBuilder HitBoxCallbackIds(List<int> callbackIds);
        }
    }
}