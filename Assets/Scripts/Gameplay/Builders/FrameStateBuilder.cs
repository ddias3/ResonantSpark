using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class FrameStateBuilder : IFrameStateBuilder {
            public List<int> hitBoxCallbackIds { get; private set; }
            public bool activateHitBox { get; private set; }

            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }

            public FrameState Build(Dictionary<int, HitBox> hitBoxMap) {
                List<HitBox> hitBoxList = new List<HitBox>();

                for (int n = 0; n < hitBoxCallbackIds.Count; ++n) {
                    hitBoxList.Add(hitBoxMap[hitBoxCallbackIds[n]]);
                }

                return new FrameState(hitBoxList, activateHitBox, chainCancellable, specialCancellable, hitDamage, blockDamage, hitStun, blockStun);
            }

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