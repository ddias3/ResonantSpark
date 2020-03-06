using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Utility {
        public partial class FrameListBuilder : IFrameListCallbackObj {

            // could've used a Tuple
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

                public List<int> hitBoxCallbackIds;

                public int _counter = FrameUtil.Default.counter++;

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

            public (List<FrameStateBuilder>, Dictionary<int, Action<IHitBoxCallbackObject>>) CreateFrameList() {
                (List<ListStackFrame> completeStackFrames, Dictionary<int, Action<IHitBoxCallbackObject>> hitBoxCallbackMaps) = ReadInput(this.entries);
                List<FrameStateBuilder> frameList = CreateFromCompleteStackFrames(completeStackFrames);
                return (frameList, hitBoxCallbackMaps);
            }

            private (List<ListStackFrame>, Dictionary<int, Action<IHitBoxCallbackObject>>) ReadInput(List<FrameUtilMapObject> entries) {
                bool chainCancellable = FrameUtil.Default.chainCancellable;
                bool specialCancellable = FrameUtil.Default.specialCancellable;
                int hitDamage = FrameUtil.Default.hitDamage;
                int blockDamage = FrameUtil.Default.blockDamage;
                float hitStun = FrameUtil.Default.hitStun;
                float blockStun = FrameUtil.Default.blockStun;

                int hitBoxCallbackCounter = 0;
                Dictionary<int, Action<IHitBoxCallbackObject>> hitBoxCallbackMap = new Dictionary<int, Action<IHitBoxCallbackObject>>();

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
                    blockStun = blockStun,
                    hitBoxCallbackIds = new List<int>(),
                };
                stack.Push(baseStackFrame);

                foreach (FrameUtilMapObject entry in entries) {
                    switch (entry.option) {
                        case "startFrame":
                            topStackFrame = new ListStackFrame {
                                startFrame = ((int)entry.content) - 1,
                                endFrame = -1,
                                chainCancellable = chainCancellable,
                                specialCancellable = specialCancellable,
                                hitDamage = hitDamage,
                                blockDamage = blockDamage,
                                hitStun = hitStun,
                                blockStun = blockStun,
                                hitBoxCallbackIds = new List<int>(),
                            };
                            stack.Push(topStackFrame);
                            break;
                        case "endFrame":
                            do {
                                topStackFrame.endFrame = ((int)entry.content) - 1;
                                completeStackFrames.Add(stack.Pop());
                                topStackFrame = stack.Peek();
                            } while (!System.Object.ReferenceEquals(topStackFrame, baseStackFrame));
                            break;
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
                            topStackFrame.blockDamage = blockDamage = (int)entry.content;
                            break;
                        case "hitStun":
                            topStackFrame.hitStun = hitStun = (float)entry.content;
                            break;
                        case "blockStun":
                            topStackFrame.blockStun = blockStun = (float)entry.content;
                            break;
                        case "hitBox":
                            topStackFrame.hitBoxCallbackIds = new List<int>();
                            if (entry.content != null) {
                                hitBoxCallbackMap.Add(hitBoxCallbackCounter, (Action<IHitBoxCallbackObject>)entry.content);
                                topStackFrame.hitBoxCallbackIds.Add(hitBoxCallbackCounter);

                                hitBoxCallbackCounter++;
                            }
                            break;
                    }
                }

                if (!System.Object.ReferenceEquals(stack.Peek(), baseStackFrame)) {
                    throw new InvalidProgramException("Frame List inputs are missing an end frame for list creation");
                }

                baseStackFrame.endFrame = completeStackFrames[completeStackFrames.Count - 1].endFrame;

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

                completeStackFrames.Insert(0, baseStackFrame);

                return (completeStackFrames, hitBoxCallbackMap);
            }

            private List<FrameStateBuilder> CreateFromCompleteStackFrames(List<ListStackFrame> completeStackFrames) {
                int numFrames = completeStackFrames[0].endFrame;

                List<FrameStateBuilder> frameList = new List<FrameStateBuilder>(numFrames);
                for (int n = 0; n < numFrames; ++n) {
                    frameList.Add(new FrameStateBuilder());
                }

                foreach (ListStackFrame stackFrame in completeStackFrames) {
                    for (int frame = stackFrame.startFrame; frame < stackFrame.endFrame; ++frame) {
                        frameList[frame]
                            .SupplyAllStaticInfo(stackFrame.chainCancellable, stackFrame.specialCancellable, stackFrame.hitDamage, stackFrame.blockDamage, stackFrame.hitStun, stackFrame.blockStun)
                            .HitBoxCallbackIds(stackFrame.hitBoxCallbackIds);
                    }
                }

                return frameList;
            }
        }
    }
}