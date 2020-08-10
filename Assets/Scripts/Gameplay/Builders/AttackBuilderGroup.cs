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
        public partial class AttackBuilderGroup : IAttackCallbackGroupObj {
            private Dictionary<string, bool> valuesSet;

            private List<FrameStateBuilder> frames;
            private Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap;

            private List<FrameState> builtFrameStates;
            private List<Hit> builtHits;

            private AllServices services;

            public AttackBuilderGroup(AllServices services) {
                this.services = services;

                valuesSet = new Dictionary<string, bool> {
                    { "move", false },
                    { "framesContinuous", false },
                    { "cleanUpCallback", false },
                    { "animStateName", false },
                    { "initAttackState", false },
                    { "frames", false },
                };

                moveX = null;
                moveY = null;
                moveZ = null;
                framesContinuous = null;
                cleanUpCallback = null;
                animStateName = null;
                initAttackState = null;

                frames = new List<FrameStateBuilder>();
            }

            public void BuildAttack() {
                if (ValueSet("frames")) {
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
            }

            public bool ValueSet(string value) {
                return valuesSet[value];
            }

            public List<FrameState> GetFrames() {
                return builtFrameStates;
            }

            public void SetFrames(List<FrameState> frames) {
                this.builtFrameStates = frames;
            }

            public List<Hit> GetHits() {
                return builtHits;
            }

            public void SetHits(List<Hit> hits) {
                this.builtHits = hits;
            }
        }
    }
}