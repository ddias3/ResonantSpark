using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class FrameStateBuilder : IFrameStateBuilder {

            public FrameState Build(Dictionary<int, Hit> hitMap) {
                List<Hit> hitList = new List<Hit>();

                for (int n = 0; n < hitCallbackIds.Count; ++n) {
                    hitList.Add(hitMap[hitCallbackIds[n]]);
                }

                return new FrameState(
                    chainCancellable,
                    specialCancellable,
                    cancellableOnWhiff,
                    hitList,
                    armorCallback,
                    trackCallback,
                    soundClip,
                    soundCallback,
                    projectile,
                    projectileCallback);
            }
        }
    }
}