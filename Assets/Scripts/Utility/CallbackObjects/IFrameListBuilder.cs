using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameListCallbackObj {
            IFrameListCallbackObj SpecialCancellable(bool specialCancellable);
            IFrameListCallbackObj ChainCancellable(bool chainCancellable);
            IFrameListCallbackObj CancellableOnWhiff(bool cancellableOnWhiff);
            IFrameListCallbackObj Hit(Action<IHitCallbackObject> callback);
            IFrameListCallbackObj Track(Action<Vector3, Transform> callback);
            IFrameListCallbackObj Sound(AudioClip audioClip, Action<AudioResource> soundCallback);
            IFrameListCallbackObj Projectile(Projectile projectile, Action<Projectile> projectileCallback);
            IFrameListCallbackObj From(int startFrame);
            IFrameListCallbackObj To(int endFrame);
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

            public IFrameListCallbackObj Hit(Action<IHitCallbackObject> hitCallback) {
                entries.Add(new FrameUtilMapObject {
                    option = "hit",
                    content = hitCallback
                });
                return this;
            }

            public IFrameListCallbackObj Track(Action<Vector3, Transform> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "track",
                    content = callback
                });
                return this;
            }

            public IFrameListCallbackObj Sound(AudioClip clip, Action<AudioResource> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "sound",
                    content = (clip, callback)
                });
                return this;
            }

            public IFrameListCallbackObj Projectile(Projectile projectile, Action<Projectile> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "projectile",
                    content = (projectile, callback)
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
