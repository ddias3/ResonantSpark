using System;

namespace ResonantSpark {
    namespace Utility {
        public class AttackTracker {

            private int startFrame;
            private int frameCount;

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

            public int GetFrameCount() {
                return frameCount++;
            }
        }
    }
}