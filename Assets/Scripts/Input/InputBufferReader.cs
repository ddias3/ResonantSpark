using System;

namespace ResonantSpark {
    namespace Input {
        public class InputBufferReader {
            private GameInputStruct[] inputBuffer;

            private int inputBufferSize;
            private int inputIndex;
            private int inputDelay;
            private int bufferLength;

            private int bufferIndex;

            public InputBufferReader(GameInputStruct[] inputBuffer, int inputBufferSize, int inputIndex, int inputDelay, int bufferLength) {
                this.inputBuffer = inputBuffer;
                this.inputBufferSize = inputBufferSize;
                this.inputIndex = inputIndex;
                this.inputDelay = inputDelay;
                this.bufferLength = bufferLength;

                ResetCurrIndex();
            }

            public void ResetCurrIndex() {
                bufferIndex = inputBufferSize;
            }

            public bool IsReadable() {
                return bufferIndex >= 0;
            }

            public int ReadBuffer(out GameInputStruct curr) {
                if (IsReadable()) {
                    int currIndex = (inputIndex - (inputDelay + bufferIndex) + bufferLength) % bufferLength;
                    --bufferIndex;
                    curr = inputBuffer[currIndex];
                    return currIndex;
                }
                else {
                    throw new OverflowException();
                }
            }
        }
    }
}