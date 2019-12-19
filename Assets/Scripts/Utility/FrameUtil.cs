using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    public static class FrameUtil {

        public static List<FrameState> CreateList(Action<IFrameListBuilder> callback) {
            FrameListBuilder builder = new FrameListBuilder();
            callback(builder);
            return builder.CreateFrameList();
        }

        private class FrameListBuilder : IFrameListBuilder {

            private class FrameUtilMapObject {
                public string option { get; set; }
                public object content { get; set; }
            }

            private List<FrameState> frameList;
            private List<FrameUtilMapObject> inputs;

            public FrameListBuilder() {
                inputs = new List<FrameUtilMapObject>();
            }

            public List<FrameState> CreateFrameList() {
                frameList = new List<FrameState>();

                return frameList;
            }

            public IFrameListBuilder BlockStun(float frame) {
                inputs.Add(new FrameUtilMapObject {
                    option = "blockStun",
                    content = frame
                });
                return this;
            }

            public IFrameListBuilder ChainCancellable(bool chainCancellable) {
                inputs.Add(new FrameUtilMapObject {
                    option = "chainCancellable",
                    content = chainCancellable
                });
                return this;
            }

            public IFrameListBuilder HitDamage(int damage) {
                inputs.Add(new FrameUtilMapObject {
                    option = "hitDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListBuilder BlockDamage(int damage) {
                inputs.Add(new FrameUtilMapObject {
                    option = "blockDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListBuilder From(int startFrame) {
                return this;
            }

            public IFrameListBuilder HitBox(Action<IHitBoxBuilder> callback) {
                HitBoxBuilder builder = new HitBoxBuilder();
                callback(builder);

                // TODO: Create the hitboxes and put them in the correct frames
                // TODO: Pass along the events

                return this;
            }

            public IFrameListBuilder HitBox(HitBox hitBox) {
                return this;
            }

            public IFrameListBuilder HitBox() {
                return this;
            }

            public IFrameListBuilder HitStun(float frames) {
                return this;
            }

            public IFrameListBuilder SpecialCancellable(bool specialCancellable) {
                return this;
            }

            public IFrameListBuilder To(int endFrame) {
                return this;
            }
        }
    }
}