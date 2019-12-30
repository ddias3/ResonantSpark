using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Input {
        public class InputBuffer : MonoBehaviour {

            public bool breakPoint = false;

            public int inputDelay;
            public int inputBufferSize;

            public int bufferLength;

            private FrameEnforcer frame;

            private GameInputStruct currState;
            private GameInputStruct[] inputBuffer;
            private int inputIndex = 0;

            private Input.Factory inputFactory;

            [SerializeField]
            private List<Input.Combinations.Combination> inputCombinations;

            public void Start() {
                frame = gameObject.GetComponentInParent<FrameEnforcer>();
                frame.SetUpdate(new Action<int>(ServeBuffer));

                inputFactory = new Input.Factory();
                inputBuffer = new GameInputStruct[bufferLength];
                currState = new GameInputStruct {
                    direction = FightingGameInputCodeDir.Neutral,
                    butA = false,
                    butB = false,
                    butC = false,
                    butD = false,
                    butS = false,
                };

                for (int n = 0; n < inputBuffer.Length; ++n) {
                    inputBuffer[n] = new GameInputStruct {
                        direction = FightingGameInputCodeDir.Neutral,
                        butA = false,
                        butB = false,
                        butC = false,
                        butD = false,
                        butS = false,
                    };
                }

                inputCombinations = new List<Input.Combinations.Combination>();
            }

            public void SetCurrentInputState(FightingGameInputCodeDir dirInputCode = FightingGameInputCodeDir.Neutral, int buttonInputCode = 0) {
                currState.direction = dirInputCode != FightingGameInputCodeDir.None ? dirInputCode : FightingGameInputCodeDir.Neutral;
                currState.butA = (buttonInputCode & (int) FightingGameInputCodeBut.A) != 0;
                currState.butB = (buttonInputCode & (int) FightingGameInputCodeBut.B) != 0;
                currState.butC = (buttonInputCode & (int) FightingGameInputCodeBut.C) != 0;
                currState.butD = (buttonInputCode & (int) FightingGameInputCodeBut.D) != 0;
                currState.butS = (buttonInputCode & (int) FightingGameInputCodeBut.S) != 0;
            }

            public void ServeBuffer(int frameIndex) {
                ServeInput();
                ClearInputCombinations(frameIndex);
                FindCombinations(frameIndex);
                StepFrame();
            }

            public FightingGameInputCodeDir GetLatestInput() {
                int currIndex = (bufferLength + inputIndex - inputDelay) % bufferLength;
                if (inputBuffer[currIndex].direction == FightingGameInputCodeDir.None) Debug.Break();
                return inputBuffer[currIndex].direction;
            }

            private void StepFrame() {
                inputIndex = (inputIndex + 1) % bufferLength;
            }

            private void FindCombinations(int frameIndex) {
                InputBufferReader reader = new InputBufferReader(inputBuffer, frameIndex, inputBufferSize, inputIndex, inputDelay, bufferLength);
                if (breakPoint) {
                    Debug.Log("Manual Pause");
                    Debug.Log(reader.ToDirectionText());
                    Debug.Log(reader.ToString());
                    breakPoint = false;
                }
                Input.Service.FindCombinations(reader, inputFactory, inputCombinations);
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                return inputCombinations;
            }

            public void ClearInputCombinations(int frameIndex) {
                inputCombinations = inputCombinations.Where(combo => combo.inUse || !combo.Stale(frameIndex - inputDelay)).ToList();
            }

            public void ServeInput() {
                inputBuffer[inputIndex].direction = currState.direction;
                inputBuffer[inputIndex].butA = currState.butA;
                inputBuffer[inputIndex].butB = currState.butB;
                inputBuffer[inputIndex].butC = currState.butC;
                inputBuffer[inputIndex].butD = currState.butD;
                inputBuffer[inputIndex].butS = currState.butS;
            }
        }
    }
}