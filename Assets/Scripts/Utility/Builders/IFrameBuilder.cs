using System;
using System.Collections.Generic;
using UnityEngine;


namespace ResonantSpark {
    namespace Builder {
        public interface IFrameBuilder {
            IFrameBuilder AddFrames(List<Character.FrameState> frameList);
        }
    }
}