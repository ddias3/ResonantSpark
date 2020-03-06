using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameListCallbackObj {
            IFrameListCallbackObj SpecialCancellable(bool specialCancellable);
            IFrameListCallbackObj From(int startFrame);
            IFrameListCallbackObj ChainCancellable(bool chainCancellable);
            IFrameListCallbackObj To(int endFrame);
            IFrameListCallbackObj Hit(Action<IHitCallbackObject> callback);
            IFrameListCallbackObj Hit();
            IFrameListCallbackObj HitDamage(int damage);
            IFrameListCallbackObj BlockDamage(int damage);
            IFrameListCallbackObj HitStun(float frames);
            IFrameListCallbackObj BlockStun(float frame);
            IFrameListCallbackObj CancellableOnWhiff(bool cancellableOnWhiff);
            IFrameListCallbackObj Track(Action<Transform> callback);
        }
    }

    namespace Utility {
        public partial class FrameListBuilder : IFrameListCallbackObj {

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

            public IFrameListCallbackObj CancellableOnWhiff(bool cancellableOnWhiff) {
                entries.Add(new FrameUtilMapObject {
                    option = "cancellableOnWhiff",
                    content = cancellableOnWhiff
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

            public IFrameListCallbackObj Hit(Action<IHitCallbackObject> hitBoxCallback) {
                entries.Add(new FrameUtilMapObject {
                    option = "hit",
                    content = hitBoxCallback
                });
                return this;
            }

            public IFrameListCallbackObj Hit() {
                entries.Add(new FrameUtilMapObject {
                    option = "hit",
                    content = null
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

            public IFrameListCallbackObj Track(Action<Transform> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "track",
                    content = callback
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
