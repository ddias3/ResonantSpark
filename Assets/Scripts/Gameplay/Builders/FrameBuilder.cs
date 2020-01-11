using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class FrameBuilder : IFrameCallbackObject {
            public List<FrameState> frames { get; private set; }

            public FrameBuilder() {
                frames = new List<FrameState>();
            }

            public IFrameCallbackObject AddFrames(List<FrameState> frameList) {
                frames.AddRange(frameList);
                return this;
            }

            public List<FrameState> GetFrames() {
                return frames; //.ToArray();
            }
        }
    }
}