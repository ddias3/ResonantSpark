using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    public static class FrameUtil {

        public static List<FrameState> CreateList(Action<Builder.IFrameListBuilder> callback) {
            FrameListBuilder builder = new FrameListBuilder();
            callback(builder);
            return builder.frameList;
        }

        private class FrameListBuilder : IFrameListBuilder {
            public List<FrameState> frameList { get; private set; }

            public FrameListBuilder() {
                frameList = new List<FrameState>();
            }

            public IFrameListBuilder BlockStun(float frame) {
                return this;
            }

            public IFrameListBuilder ChainCancellable(bool chainCancellable) {
                return this;
            }

            public IFrameListBuilder Damage(int damage) {
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