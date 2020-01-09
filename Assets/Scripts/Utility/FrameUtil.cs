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

            public static int counter = 0;
        }

        public static List<FrameState> CreateList(Action<IFrameListCallbackObj> callback) {
            FrameListBuilder builder = new FrameListBuilder();
            callback(builder);
            return builder.CreateFrameList();
        }

        private class FrameListBuilder : IFrameListCallbackObj {

                // could'a used a Tuple
            private struct FrameUtilMapObject {
                public string option;
                public object content;
            }

            private class ListStackFrame : IComparable<ListStackFrame> {
                public int startFrame;
                public int endFrame;

                public bool chainCancellable;
                public bool specialCancellable;
                public int hitDamage;
                public int blockDamage;
                public float hitStun;
                public float blockStun;

                public int _counter = Default.counter++;

                public int CompareTo(ListStackFrame other) {
                    if (this.startFrame < other.startFrame) {
                        return -1;
                    }
                    else if (this.startFrame == other.startFrame) {
                        if (this.endFrame < other.endFrame) {
                            return -1;
                        }
                        else if (this.endFrame == other.endFrame) {
                            return 0;
                        }
                        else {
                            return 1;
                        }
                    }
                    else {
                        return 1;
                    }
                }
            }

            private List<FrameUtilMapObject> entries;

            public FrameListBuilder() {
                entries = new List<FrameUtilMapObject>();
            }

            public List<FrameState> CreateFrameList() {
                List<ListStackFrame> completeStackFrames = ReadInput(this.entries);
                List<FrameState> frameList = CreateFromCompleteStackFrames(completeStackFrames);
                return frameList;
            }

            private List<ListStackFrame> ReadInput(List<FrameUtilMapObject> entries) {
                bool chainCancellable = Default.chainCancellable;
                bool specialCancellable = Default.specialCancellable;
                int hitDamage = Default.hitDamage;
                int blockDamage = Default.blockDamage;
                float hitStun = Default.hitStun;
                float blockStun = Default.blockStun;

                List<ListStackFrame> completeStackFrames = new List<ListStackFrame>();
                Stack<ListStackFrame> stack = new Stack<ListStackFrame>();
                ListStackFrame topStackFrame, baseStackFrame;
                baseStackFrame = topStackFrame = new ListStackFrame {
                    startFrame = 0,
                    endFrame = -1,
                    chainCancellable = chainCancellable,
                    specialCancellable = specialCancellable,
                    hitDamage = hitDamage,
                    blockDamage = blockDamage,
                    hitStun = hitStun,
                    blockStun = blockStun
                };
                stack.Push(baseStackFrame);

                foreach (FrameUtilMapObject entry in entries) {
                    switch (entry.option) {
                        case "startFrame":
                            topStackFrame = new ListStackFrame {
                                startFrame = ((int) entry.content) - 1,
                                endFrame = -1,
                                chainCancellable = chainCancellable,
                                specialCancellable = specialCancellable,
                                hitDamage = hitDamage,
                                blockDamage = blockDamage,
                                hitStun = hitStun,
                                blockStun = blockStun
                            };
                            stack.Push(topStackFrame);
                            break;
                        case "endFrame":
                            do {
                                topStackFrame.endFrame = ((int) entry.content) - 1;
                                completeStackFrames.Add(stack.Pop());
                                topStackFrame = stack.Peek();
                            } while (!Object.ReferenceEquals(topStackFrame, baseStackFrame));
                            break;
                        case "chainCancellable":
                            topStackFrame.chainCancellable = chainCancellable = (bool) entry.content;
                            break;
                        case "specialCancellable":
                            topStackFrame.specialCancellable = specialCancellable = (bool) entry.content;
                            break;
                        case "hitDamage":
                            topStackFrame.hitDamage = hitDamage = (int) entry.content;
                            break;
                        case "blockDamage":
                            topStackFrame.blockDamage = blockDamage = (int) entry.content;
                            break;
                        case "hitStun":
                            topStackFrame.hitStun = hitStun = (float) entry.content;
                            break;
                        case "blockStun":
                            topStackFrame.blockStun = blockStun = (float) entry.content;
                            break;
                    }
                }

                if (!Object.ReferenceEquals(stack.Peek(), baseStackFrame)) {
                    throw new InvalidProgramException("Frame List inputs are missing an end frame for list creation");
                }

                baseStackFrame.endFrame = completeStackFrames[completeStackFrames.Count - 1].endFrame;
                completeStackFrames.Add(baseStackFrame);

                completeStackFrames.Sort(new Comparison<ListStackFrame>((ListStackFrame x, ListStackFrame y) => {
                    if (x.startFrame < y.startFrame) {
                        return -1;
                    }
                    else if (x.startFrame == y.startFrame) {
                        if (x.endFrame < y.endFrame) {
                            return -1;
                        }
                        else if (x.endFrame == y.endFrame) {
                            return 0;
                        }
                        else {
                            return 1;
                        }
                    }
                    else {
                        return 1;
                    }
                }));

                return completeStackFrames;
            }

            private List<FrameState> CreateFromCompleteStackFrames(List<ListStackFrame> completeStackFrames) {
                int numFrames = completeStackFrames[completeStackFrames.Count - 1].endFrame;

                List<FrameState> frameList = new List<FrameState>(numFrames);
                for (int n = 0; n < numFrames; ++n) {
                    frameList.Add(new FrameState());
                }

                foreach (ListStackFrame stackFrame in completeStackFrames) {
                    for (int frame = stackFrame.startFrame; frame < stackFrame.endFrame; ++frame) {
                        FrameState curr = frameList[frame];

                        // TODO: fill frame state with correct information
                    }
                }

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