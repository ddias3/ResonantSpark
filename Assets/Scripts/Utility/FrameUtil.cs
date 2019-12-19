using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    public static class FrameUtil {

        public static List<FrameState> CreateList(Action<IFrameListCallbackObj> callback) {
            FrameListBuilder builder = new FrameListBuilder();
            callback(builder);
            return builder.CreateFrameList();
        }

        private class FrameListBuilder : IFrameListCallbackObj {

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

            public IFrameListCallbackObj BlockStun(float frame) {
                inputs.Add(new FrameUtilMapObject {
                    option = "blockStun",
                    content = frame
                });
                return this;
            }

            public IFrameListCallbackObj ChainCancellable(bool chainCancellable) {
                inputs.Add(new FrameUtilMapObject {
                    option = "chainCancellable",
                    content = chainCancellable
                });
                return this;
            }

            public IFrameListCallbackObj HitDamage(int damage) {
                inputs.Add(new FrameUtilMapObject {
                    option = "hitDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListCallbackObj BlockDamage(int damage) {
                inputs.Add(new FrameUtilMapObject {
                    option = "blockDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListCallbackObj From(int startFrame) {
                inputs.Add(new FrameUtilMapObject {
                    option = "startFrame",
                    content = startFrame
                });
                return this;
            }

            public IFrameListCallbackObj HitBox(Action<IHitBoxCallbackObject> callback) {
                HitBoxBuilder builder = new HitBoxBuilder();
                callback(builder);

                // TODO: Create the hitboxes and put them in the correct frames
                // TODO: Pass along the events

                List<HitBox> hitBoxes = builder.CreateHitBoxes();

                inputs.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = hitBoxes
                });
                return this;
            }

            public IFrameListCallbackObj HitBox(HitBox hitBox) {
                HitBox[] hitBoxes = { hitBox };
                inputs.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = new List<HitBox>(hitBoxes)
                });
                return this;
            }

            public IFrameListCallbackObj HitBox() {
                inputs.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = new List<HitBox>(0)
                });
                return this;
            }

            public IFrameListCallbackObj HitStun(float frames) {
                inputs.Add(new FrameUtilMapObject {
                    option = "hitStun",
                    content = frames
                });
                return this;
            }

            public IFrameListCallbackObj SpecialCancellable(bool specialCancellable) {
                inputs.Add(new FrameUtilMapObject {
                    option = "specialCancellable",
                    content = specialCancellable
                });
                return this;
            }

            public IFrameListCallbackObj To(int endFrame) {
                inputs.Add(new FrameUtilMapObject {
                    option = "endFrame",
                    content = endFrame
                });
                return this;
            }
        }
    }
}