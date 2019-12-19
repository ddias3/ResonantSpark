using System;
using System.Collections.Generic;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    public static class FrameUtil {

        public static class Default {
            public static bool chainCancellable = true;
            public static bool specialCancellable = true;
            public static int hitDamage = 800;
            public static int blockDamage = 0;
            public static float hitStun = 20.0f;
            public static float blockStun = 10.0f;
            public static int startFrame = 0;
        }

        public static List<FrameState> CreateList(Action<IFrameListCallbackObj> callback) {
            FrameListBuilder builder = new FrameListBuilder();
            callback(builder);
            return builder.CreateFrameList();
        }

        private class FrameListBuilder : IFrameListCallbackObj {

            private struct FrameUtilMapObject {
                public string option;
                public object content;
            }

            private struct ListStackFrame {
                public int startFrame;
                public int endFrame;

                public bool chainCancellable;
                public bool specialCancellable;
                public int hitDamage;
                public int blockDamage;
                public float hitStun;
                public float blockStun;
            }

            private List<FrameState> frameList;
            private List<FrameUtilMapObject> entries;

            public FrameListBuilder() {
                entries = new List<FrameUtilMapObject>();
            }

            private void HelperFunc(List<FrameState> frameList, int startFrame) {
                bool chainCancellable = Default.chainCancellable;
                bool specialCancellable = Default.specialCancellable;
                int hitDamage = Default.hitDamage;
                int blockDamage = Default.blockDamage;
                float hitStun = Default.hitStun;
                float blockStun = Default.blockStun;
                int endFrame = -1;
                List<HitBox> hitBoxes = null;

                foreach (FrameUtilMapObject entry in entries) {
                    switch (entry.option) {
                        case "chainCancellable":
                            chainCancellable = (bool) entry.content;
                            break;
                        case "specialCancellable":
                            specialCancellable = (bool) entry.content;
                            break;
                        case "hitDamage":
                            hitDamage = (int) entry.content;
                            break;
                        case "blockDamage":
                            blockDamage = (int) entry.content;
                            break;
                        case "startFrame":
                            startFrame = (int) entry.content;
                            break;
                        case "endFrame":
                            endFrame = (int) entry.content;
                            break;
                        case "hitBox":
                            hitBoxes = (List<HitBox>) entry.content;
                            break;
                        case "hitStun":
                            hitStun = (float) entry.content;
                            break;
                        case "blockStun":
                            blockStun = (float) entry.content;
                            break;
                    }
                }
            }

            public List<FrameState> CreateFrameList() {
                bool chainCancellable = Default.chainCancellable;
                bool specialCancellable = Default.specialCancellable;
                int hitDamage = Default.hitDamage;
                int blockDamage = Default.blockDamage;
                float hitStun = Default.hitStun;
                float blockStun = Default.blockStun;

                List<ListStackFrame> completeStackFrames = new List<ListStackFrame>();
                Stack<ListStackFrame> stack = new Stack<ListStackFrame>();
                ListStackFrame topStackFrame = new ListStackFrame {
                    startFrame = 0,
                    endFrame = -1,
                    chainCancellable = chainCancellable,
                    specialCancellable = specialCancellable,
                    hitDamage = hitDamage,
                    blockDamage = blockDamage,
                    hitStun = hitStun,
                    blockStun = blockStun
                };

                foreach (FrameUtilMapObject entry in entries) {
                    switch (entry.option) {
                        case "chainCancellable":
                            topStackFrame.chainCancellable = chainCancellable = (bool)entry.content;
                            break;
                        case "specialCancellable":
                            topStackFrame.specialCancellable = specialCancellable = (bool)entry.content;
                            break;
                        case "hitDamage":
                            topStackFrame.hitDamage = hitDamage = (int)entry.content;
                            break;
                        case "blockDamage":
                            topStackFrame.blockDamage = blockDamage = (int) entry.content;
                            break;
                        case "startFrame":
                            stack.Push(topStackFrame = new ListStackFrame {
                                startFrame = (int)entry.content,
                                endFrame = -1,
                                chainCancellable = chainCancellable,
                                specialCancellable = specialCancellable,
                                hitDamage = hitDamage,
                                blockDamage = blockDamage,
                                hitStun = hitStun,
                                blockStun = blockStun
                            });
                            break;
                        case "endFrame":
                            endFrame = (int)entry.content;
                            break;
                        case "hitBox":
                            hitBoxes = (List<HitBox>)entry.content;
                            break;
                        case "hitStun":
                            hitStun = (float)entry.content;
                            break;
                        case "blockStun":
                            blockStun = (float)entry.content;
                            break;
                    }
                }

                frameList = new List<FrameState>();

                return frameList;
            }

            public IFrameListCallbackObj ChainCancellable(bool chainCancellable) {
                entries.Add(new FrameUtilMapObject {
                    option = "chainCancellable",
                    content = chainCancellable
                });
                return this;
            }

            public IFrameListCallbackObj SpecialCancellable(bool specialCancellable) {
                entries.Add(new FrameUtilMapObject {
                    option = "specialCancellable",
                    content = specialCancellable
                });
                return this;
            }

            public IFrameListCallbackObj HitDamage(int damage) {
                entries.Add(new FrameUtilMapObject {
                    option = "hitDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListCallbackObj BlockDamage(int damage) {
                entries.Add(new FrameUtilMapObject {
                    option = "blockDamage",
                    content = damage
                });
                return this;
            }

            public IFrameListCallbackObj HitBox(Action<IHitBoxCallbackObject> callback) {
                HitBoxBuilder builder = new HitBoxBuilder();
                callback(builder);

                // TODO: Create the hitboxes and put them in the correct frames
                // TODO: Pass along the events

                List<HitBox> hitBoxes = builder.CreateHitBoxes();

                entries.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = hitBoxes
                });
                return this;
            }

            public IFrameListCallbackObj HitBox(HitBox hitBox) {
                HitBox[] hitBoxes = { hitBox };
                entries.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = new List<HitBox>(hitBoxes)
                });
                return this;
            }

            public IFrameListCallbackObj HitBox() {
                entries.Add(new FrameUtilMapObject {
                    option = "hitBox",
                    content = new List<HitBox>(0)
                });
                return this;
            }

            public IFrameListCallbackObj HitStun(float frames) {
                entries.Add(new FrameUtilMapObject {
                    option = "hitStun",
                    content = frames
                });
                return this;
            }

            public IFrameListCallbackObj BlockStun(float frame) {
                entries.Add(new FrameUtilMapObject {
                    option = "blockStun",
                    content = frame
                });
                return this;
            }

            public IFrameListCallbackObj From(int startFrame) {
                entries.Add(new FrameUtilMapObject {
                    option = "startFrame",
                    content = startFrame
                });
                return this;
            }

            public IFrameListCallbackObj To(int endFrame) {
                entries.Add(new FrameUtilMapObject {
                    option = "endFrame",
                    content = endFrame
                });
                return this;
            }
        }
    }
}