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
            private Dictionary<int, Action<IHitBoxCallbackObject>> hitBoxCallbackMap;

            private List<FrameState> builtFrameStates;
            private List<HitBox> builtHitBoxes;

            private AllServices services;

            public AttackBuilder(AllServices services) {
                this.services = services;

                frames = new List<FrameStateBuilder>();
            }

            public void BuildAttack() {
                IHitBoxService hitBoxService = services.GetService<IHitBoxService>();
                builtFrameStates = new List<FrameState>();
                builtHitBoxes = new List<HitBox>();

                Dictionary<int, HitBox> hitBoxMap = new Dictionary<int, HitBox>();

                foreach (KeyValuePair<int, Action<IHitBoxCallbackObject>> entry in hitBoxCallbackMap) {
                    Action<IHitBoxCallbackObject> callback = entry.Value;

                    HitBoxBuilder builder = new HitBoxBuilder(services);

                    callback(builder);
                    // TODO: Pass along the events

                    HitBox hitBox = builder.CreateHitBox(hitBoxService.GetEmptyHoldTransform());

                    hitBoxMap.Add(entry.Key, hitBox);
                    builtHitBoxes.Add(hitBox);
                }

                for (int n = 0; n < frames.Count; ++n) {
                    FrameStateBuilder frameStateBuilder = frames[n];
                    builtFrameStates.Add(frameStateBuilder.Build(hitBoxMap));
                }
            }

            public List<FrameState> GetFrames() {
                return builtFrameStates;
            }

            public List<HitBox> GetHitBoxes() {
                return builtHitBoxes;
            }
        }
    }
}