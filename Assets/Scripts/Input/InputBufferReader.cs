using System;

namespace ResonantSpark {
    namespace Input {
        public class InputBufferReader {
            private GameInputStruct[] inputBuffer;
            private int frameIndex;

            private int inputBufferSize;
            private int inputIndex;
            private int inputDelay;
            private int bufferLength;

            private int readerIndex;
            private int lookBehindIndex;
            private int lookAheadIndex;

            public InputBufferReader(GameInputStruct[] inputBuffer, int frameIndex, int inputBufferSize, int inputIndex, int inputDelay, int bufferLength) {
                this.inputBuffer = inputBuffer;
                this.frameIndex = frameIndex;
                this.inputBufferSize = inputBufferSize;
                this.inputIndex = inputIndex;
                this.inputDelay = inputDelay;
                this.bufferLength = bufferLength;

                ResetCurrIndex();
            }

            public int currentFrame {
                get { return frameIndex - inputDelay; }
            }

            public void ResetCurrIndex() {
                readerIndex = -1;
                lookAheadIndex = 0;
                lookBehindIndex = 0;
            }

            public void ResetLookAhead() {
                lookAheadIndex = 0;
            }

            public void ResetLookBehind() {
                lookBehindIndex = 0;
            }

            public void SetReadIndex(int val) {
                if (val < 0) {
                    readerIndex = inputBufferSize + val;
                }
                else {
                    readerIndex = val;
                }
            }

            public int GetAbsoluteCurrentFrame() {
                return frameIndex;
            }

            private bool IsReadable() {
                return readerIndex < inputBufferSize;
            }

            private bool IsLookAheadReadable() {
                return readerIndex + lookAheadIndex < inputBufferSize;
            }

            private bool IsLookBehindReadable() {
                return readerIndex + lookBehindIndex >= 0;
            }

            public bool ReadyNext() {
                readerIndex++;
                return IsReadable();
            }

            public bool ReadyNextLookAhead() {
                lookAheadIndex++;
                return IsLookAheadReadable();
            }

            public bool ReadyNextLookBehind() {
                lookBehindIndex--;
                return IsLookBehindReadable();
            }

            public int ReadBuffer(out GameInputStruct curr) {
                if (IsReadable()) {
                    lookAheadIndex = 0;
                    lookBehindIndex = 0;
                    // (inputDelay + inputBufferSize) is the relative start of the read point. Subtract the starting from the init.
                    int currIndex = (bufferLength + inputIndex - (inputDelay + inputBufferSize) + 1 + readerIndex) % bufferLength;
                    curr = inputBuffer[currIndex];
                    return frameIndex - (inputDelay + inputBufferSize) + 1 + readerIndex;
                }
                else {
                    curr = new GameInputStruct {
                        direction = FightingGameInputCodeDir.None,
                        butA = false,
                        butB = false,
                        butC = false,
                        butD = false,
                        butS = false,
                    };
                    return -1;
                }
            }

            public int LookAhead(out GameInputStruct ahead) {
                if (IsLookAheadReadable()) {
                    int currIndex = (bufferLength + inputIndex - (inputDelay + inputBufferSize) + 1 + readerIndex + lookAheadIndex) % bufferLength;
                    ahead = inputBuffer[currIndex];
                    return frameIndex - (inputDelay + inputBufferSize) + 1 + readerIndex + lookAheadIndex;
                }
                else {
                    ahead = new GameInputStruct {
                        direction = FightingGameInputCodeDir.None,
                        butA = false,
                        butB = false,
                        butC = false,
                        butD = false,
                        butS = false,
                    };
                    return -1;
                }
            }

            public int LookBehind(out GameInputStruct behind) {
                if (IsLookBehindReadable()) {
                    int currIndex = (bufferLength + inputIndex - (inputDelay + inputBufferSize) + 1 + readerIndex + lookBehindIndex) % bufferLength;
                    behind = inputBuffer[currIndex];
                    return frameIndex - (inputDelay + inputBufferSize) + 1 + readerIndex + lookBehindIndex;
                }
                else {
                    behind = new GameInputStruct {
                        direction = FightingGameInputCodeDir.None,
                        butA = false,
                        butB = false,
                        butC = false,
                        butD = false,
                        butS = false,
                    };
                    return -1;
                }
            }

            public string ToDirectionText() {
                System.Text.StringBuilder debugText = new System.Text.StringBuilder();
                for (int n = 0; n < inputBufferSize; ++n) {
                    int currIndex = (bufferLength + inputIndex - (inputDelay + inputBufferSize) + 1 + n) % bufferLength;
                    if (inputBuffer[currIndex].direction != FightingGameInputCodeDir.None) {
                        debugText.Append((int)inputBuffer[currIndex].direction);
                    }
                    else {
                        debugText.Append(5);
                    }
                }
                return debugText.ToString();
            }

            public new string ToString() {
                System.Text.StringBuilder toString = new System.Text.StringBuilder();
                toString.Append("Input Buffer for frame(");
                toString.Append(frameIndex);
                toString.Append("):\n");
                for (int n = 0; n < inputBufferSize; ++n) {
                    int currIndex = (bufferLength + inputIndex - (inputDelay + inputBufferSize) + 1 + n) % bufferLength;
                    toString.Append(inputBuffer[currIndex].ToString());
                    toString.Append("\n");
                }
                return toString.ToString();
            }
        }
    }
}