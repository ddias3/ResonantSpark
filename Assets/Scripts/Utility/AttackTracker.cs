using System;

namespace ResonantSpark {
    namespace Utility {
        public class AttackTracker {

            public int frameCount { get; private set; }

            private int startFrame;

            public AttackTracker() {
                startFrame = -1;
                frameCount = 0;
            }

            public void Track(int frameIndex) {
                startFrame = frameIndex;
                frameCount = 0;
            }

            public void Track() {
                frameCount = 0;
            }

            public void Increment() {
                frameCount++;
            }
        }
    }
}