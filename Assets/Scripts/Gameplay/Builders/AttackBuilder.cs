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
        public class AttackBuilder : IAttackCallbackObj {
            public string name { get; private set; }
            public Orientation orientation { get; private set; }
            public GroundRelation groundRelation { get; private set; }
            public InputNotation input { get; private set; }
            public string animStateName { get; private set; }

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

            public IAttackCallbackObj Name(string name) {
                this.name = name;
                return this;
            }
            public IAttackCallbackObj Orientation(Orientation orientation) {
                this.orientation = orientation;
                return this;
            }
            public IAttackCallbackObj GroundRelation(GroundRelation groundRelation) {
                this.groundRelation = groundRelation;
                return this;
            }
            public IAttackCallbackObj Input(InputNotation input) {
                this.input = input;
                return this;
            }
            public IAttackCallbackObj AnimationState(string animStateName) {
                this.animStateName = animStateName;
                return this;
            }
            public IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitBoxCallbackObject>> hitBoxCallbackMap) framesInfo) {
                this.frames.AddRange(framesInfo.frameList);
                this.hitBoxCallbackMap = framesInfo.hitBoxCallbackMap;
                return this;
            }
        }
    }
}