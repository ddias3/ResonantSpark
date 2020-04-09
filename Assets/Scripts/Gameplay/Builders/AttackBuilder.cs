using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class AttackBuilder : IAttackCallbackObj {
            private List<FrameStateBuilder> frames;
            private Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap;

            private List<FrameState> builtFrameStates;
            private List<Hit> builtHits;

            private AllServices services;

            public AttackBuilder(AllServices services) {
                this.services = services;

                frames = new List<FrameStateBuilder>();
            }

            public void BuildAttack() {
                builtFrameStates = new List<FrameState>();
                builtHits = new List<Hit>();

                Dictionary<int, Hit> hitMap = new Dictionary<int, Hit>();

                foreach (KeyValuePair<int, Action<IHitCallbackObject>> entry in hitCallbackMap) {
                    Action<IHitCallbackObject> callback = entry.Value;

                    HitBuilder builder = new HitBuilder(services);

                    callback(builder);

                    Hit hit = builder.CreateHit();

                    hitMap.Add(entry.Key, hit);
                    builtHits.Add(hit);
                }

                for (int n = 0; n < frames.Count; ++n) {
                    FrameStateBuilder frameStateBuilder = frames[n];
                    builtFrameStates.Add(frameStateBuilder.Build(hitMap));
                }
            }

            public List<FrameState> GetFrames() {
                return builtFrameStates;
            }

            public List<Hit> GetHits() {
                return builtHits;
            }
        }
    }
}