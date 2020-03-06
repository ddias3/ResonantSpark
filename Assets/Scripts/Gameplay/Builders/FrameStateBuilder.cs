using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {

            public FrameState Build(Dictionary<int, HitBox> hitBoxMap) {
                List<HitBox> hitBoxList = new List<HitBox>();

                for (int n = 0; n < hitBoxCallbackIds.Count; ++n) {
                    hitBoxList.Add(hitBoxMap[hitBoxCallbackIds[n]]);
                }

                return new FrameState(hitBoxList, activateHitBox, chainCancellable, specialCancellable, hitDamage, blockDamage, hitStun, blockStun);
            }
        }
    }
}