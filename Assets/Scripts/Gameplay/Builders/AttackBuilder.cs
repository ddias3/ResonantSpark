using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class AttackBuilder : IAttackBuilder {
            public string name { get; private set; }
            public Orientation orientation { get; private set; }
            public Input.InputNotation input { get; private set; }
            public string animStateName { get; private set; }
            private FrameBuilder frameBuilder;

            public FrameState[] GetFrames() {
                return frameBuilder.GetFrames();
            }

            public IAttackBuilder Name(string name) {
                this.name = name;
                return this;
            }
            public IAttackBuilder Orientation(Orientation orientation) {
                this.orientation = orientation;
                return this;
            }
            public IAttackBuilder Input(Input.InputNotation input) {
                this.input = input;
                return this;
            }
            public IAttackBuilder AnimationState(string animStateName) {
                this.animStateName = animStateName;
                return this;
            }
            public IAttackBuilder Frames(Action<IFrameBuilder> callback) {
                FrameBuilder frameBuilder = new FrameBuilder();
                callback(frameBuilder);
                this.frameBuilder = frameBuilder;
                return this;
            }
        }
    }
}