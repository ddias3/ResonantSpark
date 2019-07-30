using System.Collections;
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

    public class InputBuffer : MonoBehaviour {

        [System.Serializable]
        public struct InputStruct {
            public FightingGameInputCodeDir direction;
            public FightingGameInputCodeBut[] buttons;
        };

        public FightingGameCharacter character {
            set { controlChar = value; }
        }

        public bool breakPoint = false;

        public int inputDelay;
        public int inputBufferSize;

        public int bufferLength;

        private FightingGameCharacter controlChar;

        private InputStruct[] inputBuffer;
        private int inputIndex = 0;

        private Input.Factory inputFactory;

        private FightingGameInputCodeDir[] findCombinationsBuffer;

        [SerializeField]
        private List<Input.Combinations.Combination> inputCombinations;

        public void Start() {
            inputFactory = new Input.Factory();
            inputBuffer = new InputStruct[bufferLength];

            for (int n = 0; n < inputBuffer.Length; ++n) {
                inputBuffer[n].buttons = new FightingGameInputCodeBut[5];
            }

            findCombinationsBuffer = new FightingGameInputCodeDir[inputBufferSize + 1];
            inputCombinations = new List<Input.Combinations.Combination>();

            Debug.Break();
        }

        private float frameTime = 1f / 60.0f; // 1 sec over 60 frames
        private float elapsedTime = 0f;

        void Update() {
                // TODO: Change StepFrame to step when the next frame is closer, not only after a certain amount of time has passed.
            while (elapsedTime > frameTime) {
                FindCombinations();
                ServeInput();
                StepFrame();
            }

            elapsedTime += Time.deltaTime;
        }

        private void StepFrame() {
            inputIndex = (inputIndex + 1) % bufferLength;
            elapsedTime -= frameTime;
        }

        private void FindCombinations() {
            int currIndex;

            for (int n = 0; n <= inputBufferSize; ++n) {
                currIndex = (inputIndex - inputDelay - n + bufferLength) % bufferLength;
                if (inputBuffer[currIndex].direction != FightingGameInputCodeDir.None) {
                    findCombinationsBuffer[n] = inputBuffer[currIndex].direction;
                }
                else {
                    findCombinationsBuffer[n] = FightingGameInputCodeDir.Neutral;
                }
            }

            if (breakPoint) {
                Debug.Log("Manual Pause");
                breakPoint = false;
            }

            Input.Service.FindCombinations(findCombinationsBuffer, inputFactory, inputCombinations);
        }

        private void ServeInput() {
            controlChar.ServeInput(in inputCombinations);
        }

        public void AddInput(FightingGameInputCodeDir dirInputCode = FightingGameInputCodeDir.Neutral, FightingGameInputCodeBut buttonInputCode = FightingGameInputCodeBut.None, int layer = 0) {
            inputBuffer[inputIndex].direction = dirInputCode;
            inputBuffer[inputIndex].buttons[0] = buttonInputCode;
        }
    }
}