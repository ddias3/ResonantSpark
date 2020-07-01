using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameListCallbackObj {
            IFrameListCallbackObj SpecialCancellable(bool specialCancellable);
            IFrameListCallbackObj ChainCancellable(bool chainCancellable);
            IFrameListCallbackObj CancellableOnWhiff(bool cancellableOnWhiff);
            IFrameListCallbackObj CounterHit(bool counterHit);
            IFrameListCallbackObj Hit(Action<IHitCallbackObject> callback);
            IFrameListCallbackObj Track(Action<TargetFG> callback);
            IFrameListCallbackObj Armor(Action<HitInfo> callback);
            IFrameListCallbackObj Sound(AudioClip audioClip, Action<AudioResource> soundCallback);
            IFrameListCallbackObj Projectile(Projectile projectile, Action<Projectile> projectileCallback);
            IFrameListCallbackObj ChangeState(CharacterStates.CharacterBaseState charState);
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

            public IFrameListCallbackObj CounterHit(bool counterHit) {
                entries.Add(new FrameUtilMapObject {
                    option = "counterHit",
                    content = counterHit
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

            public IFrameListCallbackObj Track(Action<TargetFG> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "track",
                    content = callback
                });
                return this;
            }

            public IFrameListCallbackObj Armor(Action<HitInfo> callback) {
                entries.Add(new FrameUtilMapObject {
                    option = "armor",
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

            public IFrameListCallbackObj ChangeState(CharacterStates.CharacterBaseState charState) {
                entries.Add(new FrameUtilMapObject {
                    option = "changeState",
                    content = charState
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
