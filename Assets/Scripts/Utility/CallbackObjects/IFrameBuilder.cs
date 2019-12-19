using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameCallbackObject {
            IFrameCallbackObject AddFrames(List<Character.FrameState> frameList);
        }
    }
}