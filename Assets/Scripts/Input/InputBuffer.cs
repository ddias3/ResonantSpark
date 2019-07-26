using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour {

    public FightingGameCharacter character;

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

    [System.Serializable]
    public struct CharMoveStruct {
        public FightingGameInputCodeDir direction;
        public FightingGameInputCodeBut[] buttons;
    };

    public int inputDelay;
    public int inputBufferSize;

    public int bufferLength;

    private CharMoveStruct[] inputBuffer;
    private int inputIndex = 0;

    // Start is called before the first frame update
    void Start() {
        inputBuffer = new CharMoveStruct[bufferLength];

        for (int n = 0; n < inputBuffer.Length; ++n) {
            inputBuffer[n].buttons = new FightingGameInputCodeBut[5];
        }
    }

    private float frameTime = 1f / 60.0f; // 1 sec over 60 frames
    private float elapsedTime = 0f;

    void Update() {
        while (elapsedTime > frameTime) {
            ServeInput();
            StepFrame();
        }

        elapsedTime += Time.deltaTime;
    }

    private void StepFrame() {
        inputIndex = (inputIndex + 1) % bufferLength;
        elapsedTime -= frameTime;
    }

    private void ServeInput() {
        int curIndex;

        for (int n = 0; n <= inputBufferSize; ++n) {
            curIndex = (inputIndex - inputDelay - n + bufferLength) % bufferLength;

            if (inputBuffer[(curIndex == 0) ? bufferLength - 1 : curIndex - 1].direction == FightingGameInputCodeDir.Neutral &&
                inputBuffer[curIndex].direction != FightingGameInputCodeDir.Neutral) {
                switch (inputBuffer[curIndex].direction) {
                    case FightingGameInputCodeDir.Down:
                        character.PerformAction("stepSpine");
                        break;
                    case FightingGameInputCodeDir.Left:
                        character.PerformAction("stepBackward");
                        break;
                    case FightingGameInputCodeDir.Right:
                        character.PerformAction("stepForward");
                        break;
                    case FightingGameInputCodeDir.Up:
                        character.PerformAction("stepChest");
                        break;
                }
            }
        }
    }

    public void AddInput(FightingGameInputCodeDir dirInputCode = FightingGameInputCodeDir.Neutral, FightingGameInputCodeBut buttonInputCode = FightingGameInputCodeBut.None, int layer = 0) {
        inputBuffer[inputIndex].direction = dirInputCode;
        inputBuffer[inputIndex].buttons[0] = buttonInputCode;
    }
}
