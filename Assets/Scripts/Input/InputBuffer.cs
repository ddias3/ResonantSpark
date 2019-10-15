using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {

    [System.Serializable]
    public enum FightingGameInputCodeDir : int {
        // [7,8,9]       [A] [B] [S]
        // [4,5,6]       [D] [C]
        // [1,2,3]

        None = 0,

        DownLeft,
        Down,
        DownRight,
        Left,
        Neutral,
        Right,
        UpLeft,
        Up,
        UpRight,
    };

    [System.Serializable]
    public enum FightingGameInputCodeBut : int {
        // [7,8,9]       [A] [B] [S]
        // [4,5,6]       [D] [C]
        // [1,2,3]

        None = 0,

        A,
        B,
        C,
        D,
        S
    };

    [RequireComponent(typeof(FrameEnforcer))]
    public class InputBuffer : MonoBehaviour {

        [System.Serializable]
        public struct InputStruct {
            public FightingGameInputCodeDir direction;
            public FightingGameInputCodeBut[] buttons;
        };

        public bool breakPoint = false;

        public int inputDelay;
        public int inputBufferSize;

        public int bufferLength;

        private FrameEnforcer frame;

        private InputStruct[] inputBuffer;
        private int inputIndex = 0;

        private Input.Factory inputFactory;

        private StringBuilder findCombinationsBuffer;

        //[SerializeField]
        private List<Input.Combinations.Combination> inputCombinations;

        public void Start() {
            frame = gameObject.GetComponent<FrameEnforcer>();
            frame.SetUpdate(new Action<int>(ServeBuffer));

            inputFactory = new Input.Factory();
            inputBuffer = new InputStruct[bufferLength];

            for (int n = 0; n < inputBuffer.Length; ++n) {
                inputBuffer[n].buttons = new FightingGameInputCodeBut[5];
            }

            findCombinationsBuffer = new StringBuilder(inputBufferSize + 1);
            inputCombinations = new List<Input.Combinations.Combination>();
        }

        public void ServeBuffer(int frameIndex) {
            ClearInputCombinations(frameIndex);
            FindCombinations(frameIndex);
            StepFrame();
        }
        
        public FightingGameInputCodeDir GetLatestInput() {
            int currIndex = (inputIndex - inputDelay + bufferLength) % bufferLength;
            if (inputBuffer[currIndex].direction != FightingGameInputCodeDir.None) {
                return inputBuffer[currIndex].direction;
            }
            else {
                return FightingGameInputCodeDir.Neutral;
            }
        }

        private void StepFrame() {
            inputIndex = (inputIndex + 1) % bufferLength;
        }

        private void FindCombinations(int frameIndex) {
            if (breakPoint) {
                Debug.Log("Manual Pause");
                breakPoint = true;
            }

            int currIndex;
            findCombinationsBuffer.Clear();

            for (int n = inputBufferSize; n >= 0; --n) {
                currIndex = (inputIndex - (inputDelay + n) + bufferLength) % bufferLength;
                if (inputBuffer[currIndex].direction != FightingGameInputCodeDir.None) {
                    findCombinationsBuffer.Append((int) inputBuffer[currIndex].direction);
                }
                else {
                    findCombinationsBuffer.Append(5);
                }
            }

            if (breakPoint) {
                Debug.Log("Manual Pause");
                breakPoint = false;
            }

            Input.Service.FindCombinations(findCombinationsBuffer.ToString(), inputFactory, frameIndex, inputCombinations);
        }

        public List<Input.Combinations.Combination> GetFoundCombinations() {
            return inputCombinations;
        }

        public void ClearInputCombinations(int frameIndex) {
            inputCombinations = inputCombinations.Where(combo => combo.inUse && !combo.Stale(frameIndex)).ToList();
        }

        public void AddInput(FightingGameInputCodeDir dirInputCode = FightingGameInputCodeDir.Neutral, FightingGameInputCodeBut buttonInputCode = FightingGameInputCodeBut.None, int layer = 0) {
            inputBuffer[inputIndex].direction = dirInputCode;
            inputBuffer[inputIndex].buttons[0] = buttonInputCode;
        }
    }
}