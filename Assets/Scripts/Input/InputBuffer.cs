using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

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

            private Combinations.DirectionCurrent currDirection;
            private Combinations.ButtonsCurrent currButtons;

            [SerializeField]
            private List<Input.Combinations.Combination> foundInputCombinations;
            [SerializeField]
            private List<Input.Combinations.Combination> servedInputCombinations;
            [SerializeField]
            private List<Input.Combinations.Combination> inUseInputCombinations;

            public void Awake() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.InputBuffer, new Action<int>(ServeBuffer));

                inputFactory = new Input.Factory();
                currState = new GameInputStruct {
                    direction = FightingGameAbsInputCodeDir.Neutral,
                    butA = false,
                    butB = false,
                    butC = false,
                    butD = false,
                    butS = false,
                };

                ResetBuffer();

                currDirection = inputFactory.CreateCombination<Combinations.DirectionCurrent>(0);
                currButtons = inputFactory.CreateCombination<Combinations.ButtonsCurrent>(0);

                currDirection.Init(0, FightingGameAbsInputCodeDir.Neutral);
                currButtons.Init(0, false, false, false, false, false);

                foundInputCombinations = new List<Input.Combinations.Combination>();
                servedInputCombinations = new List<Input.Combinations.Combination>();
                inUseInputCombinations = new List<Input.Combinations.Combination>();
            }

            public void ResetBuffer() {
                inputBuffer = new GameInputStruct[bufferLength];
                for (int n = 0; n < inputBuffer.Length; ++n) {
                    inputBuffer[n] = new GameInputStruct {
                        direction = FightingGameAbsInputCodeDir.Neutral,
                        butA = false,
                        butB = false,
                        butC = false,
                        butD = false,
                        butS = false,
                    };
                }
            }

            public void SetCurrentInputState(FightingGameAbsInputCodeDir dirInputCode = FightingGameAbsInputCodeDir.Neutral, int buttonInputCode = 0) {
                currState.direction = dirInputCode != FightingGameAbsInputCodeDir.None ? dirInputCode : FightingGameAbsInputCodeDir.Neutral;
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

            public FightingGameAbsInputCodeDir GetLatestInput() {
                int currIndex = (bufferLength + inputIndex - inputDelay) % bufferLength;
                if (inputBuffer[currIndex].direction == FightingGameAbsInputCodeDir.None) Debug.Break();
                return inputBuffer[currIndex].direction;
            }

            private void StepFrame() {
                inputIndex = (inputIndex + 1) % bufferLength;
            }

            private void FindCombinations(int frameIndex) {
                    // TODO: Change the reader to not be instantiated every frame.
                InputBufferReader reader = new InputBufferReader(inputBuffer, frameIndex, inputBufferSize, inputIndex, inputDelay, bufferLength);
                if (Keyboard.current.backspaceKey.wasPressedThisFrame) {
                    breakPoint = true;
                }
                if (breakPoint) {
                    Debug.Log("Manual Pause");
                    Debug.Log(reader.ToDirectionText());
                    Debug.Log(reader.ToString());
                    Debug.Break();
                    //breakPoint = false;
                }

                currDirection = Input.Service.FindDirectionCurrent(reader, inputFactory, servedInputCombinations, foundInputCombinations);
                currButtons = Input.Service.FindButtonsCurrent(reader, inputFactory, servedInputCombinations, foundInputCombinations);

                Input.Service.FindCombinations(reader, inputFactory, servedInputCombinations, foundInputCombinations);

                //int counter = 0;
                //inputCombinations.ForEach(combo => {
                //    if (combo.Stale(reader.currentFrame)) {
                //        ++counter;
                //        Debug.LogWarning("Frame(" + frameIndex + ") of type " + combo.GetType() + " at index " + combo.GetFrame());
                //    }
                //});
                //if (counter > 0) {
                //    Debug.LogWarning("FindCombinations added " + counter + " stale inputs");
                //}
            }

            public void ForEach(Action<Combinations.Combination> callback) {
                for (int n = 0; n < foundInputCombinations.Count; ++n) {
                    callback(foundInputCombinations[n]);
                }

                //callback(currDirection);
                //callback(currButtons);
            }

            public void Use(Combinations.Combination combo) {
                combo.inUse = true;

                inUseInputCombinations.Add(combo);
                inUseInputCombinations.Sort();

                if (combo.GetType() != typeof(Input.Combinations.DirectionCurrent)
                    && combo.GetType() != typeof(Input.Combinations.ButtonsCurrent)) {
                    servedInputCombinations.Add(combo);
                    servedInputCombinations.Sort();

                    foundInputCombinations.Remove(combo);
                }
            }

            public void RemoveInUseCombinations(List<Combinations.Combination> combos) {
                for (int n = 0; n < combos.Count; ++n) {
                    combos[n].inUse = false;
                    inUseInputCombinations.Remove(combos[n]);
                }
            }

            public List<Combinations.Combination> GetInUseCombinations() {
                return inUseInputCombinations;
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                return foundInputCombinations;
            }

            public void ClearInputCombinations(int frameIndex) {
                foundInputCombinations.RemoveAll(combo => combo.Stale(frameIndex - inputDelay));
                servedInputCombinations.RemoveAll(combo => (frameIndex - combo.GetFrame()) > inputBufferSize);
                inUseInputCombinations.RemoveAll(combo => (frameIndex - combo.GetFrame()) > inputBufferSize);
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