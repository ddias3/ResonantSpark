using System;

namespace ResonantSpark {
    namespace Utility {
        public class Tracker {

            public int frameCount { get; private set; }

            private readonly int numFrames;
            private Action callback;

            public Tracker(int numFrames, Action callback) {
                frameCount = 0;
                this.numFrames = numFrames;
                this.callback = callback;
            }

            public void Track() {
                frameCount = 0;
            }

            public void Increment() {
                if (frameCount >= numFrames) {
                    callback?.Invoke();
                }
                else {
                    frameCount++;
                }
            }
        }
    }
}
