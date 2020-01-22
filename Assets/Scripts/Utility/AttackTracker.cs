using System;

namespace ResonantSpark {
    namespace Utility {
        public class AttackTracker {

            public int frameCount { get; private set; }

            private int startFrame;

            private readonly int numFrames;

            public AttackTracker(int numFrames) {
                startFrame = -1;
                frameCount = 0;
                this.numFrames = numFrames;
            }

            public void Track(int frameIndex) {
                startFrame = frameIndex;
                frameCount = 0;
            }

            public void Increment() {
                frameCount++;
            }
        }
    }
}