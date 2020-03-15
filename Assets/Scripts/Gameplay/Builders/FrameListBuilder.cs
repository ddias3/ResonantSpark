using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Gameplay;

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
                public bool cancellableOnWhiff;

                public List<int> hitCallbackIds;

                public Action<Vector3, Transform> trackCallback = null;

                public AudioClip soundClip = null;
                public Action<AudioResource> soundCallback = null;

                public Projectile projectile = null;
                public Action<Projectile> projectileCallback = null;

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

            public (List<FrameStateBuilder>, Dictionary<int, Action<IHitCallbackObject>>) CreateFrameList() {
                (List<ListStackFrame> completeStackFrames, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMaps) = ReadInput(this.entries);
                List<FrameStateBuilder> frameList = CreateFromCompleteStackFrames(completeStackFrames);
                return (frameList, hitCallbackMaps);
            }

            private (List<ListStackFrame>, Dictionary<int, Action<IHitCallbackObject>>) ReadInput(List<FrameUtilMapObject> entries) {
                bool chainCancellable = FrameUtil.Default.chainCancellable;
                bool specialCancellable = FrameUtil.Default.specialCancellable;
                bool cancellableOnWhiff = FrameUtil.Default.cancellableOnWhiff;

                int hitCallbackCounter = 0;
                Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap = new Dictionary<int, Action<IHitCallbackObject>>();

                List<ListStackFrame> completeStackFrames = new List<ListStackFrame>();
                Stack<ListStackFrame> stack = new Stack<ListStackFrame>();
                ListStackFrame topStackFrame, baseStackFrame;
                baseStackFrame = topStackFrame = new ListStackFrame {
                    startFrame = 0,
                    endFrame = -1,
                    chainCancellable = chainCancellable,
                    specialCancellable = specialCancellable,
                    cancellableOnWhiff = cancellableOnWhiff,
                    hitCallbackIds = new List<int>(),
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
                                cancellableOnWhiff = cancellableOnWhiff,
                                hitCallbackIds = new List<int>(),
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
                        case "cancellableOnWhiff":
                            topStackFrame.cancellableOnWhiff = cancellableOnWhiff = (bool)entry.content;
                            break;
                        case "track":
                            topStackFrame.trackCallback = (Action<Vector3, Transform>)entry.content;
                            break;
                        case "sound":
                            topStackFrame.soundClip = (((AudioClip, Action<AudioResource>))entry.content).Item1;
                            topStackFrame.soundCallback = (((AudioClip, Action<AudioResource>))entry.content).Item2;
                            break;
                        case "projectile":
                            topStackFrame.projectile = (((Projectile, Action<Projectile>))entry.content).Item1;
                            topStackFrame.projectileCallback = (((Projectile, Action<Projectile>))entry.content).Item2;
                            break;
                        case "hit":
                            topStackFrame.hitCallbackIds = new List<int>();
                            if (entry.content != null) {
                                hitCallbackMap.Add(hitCallbackCounter, (Action<IHitCallbackObject>)entry.content);
                                topStackFrame.hitCallbackIds.Add(hitCallbackCounter);

                                hitCallbackCounter++;
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

                return (completeStackFrames, hitCallbackMap);
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
                            .SupplyAllStaticInfo(stackFrame.chainCancellable, stackFrame.specialCancellable, stackFrame.cancellableOnWhiff)
                            .HitCallbackIds(stackFrame.hitCallbackIds);
                    }
                }

                return frameList;
            }
        }
    }
}