using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using ResonantSpark.Input;

public class TestInput : MonoBehaviour {

    [Serializable]
    public class GameInputStructWrapper {
        public FightingGameInputCodeDir[] direction;
        public bool[] buttonA;
        public bool[] buttonB;
        public bool[] buttonC;
        public bool[] buttonD;
        public bool[] buttonS;
    }

    public bool autoInput = false;
    public FightingGameInputCodeDir[] inputString;
    public GameInputStructWrapper[] selectableStrings;
    public int test0 = 0;
    public int test1 = 1;
    public int test2 = 2;
    public int test3 = 3;
    public int test4 = 4;
    public int test5 = 5;
    public int test6 = 6;

    public InputBuffer inputBuffer;

    public Image immediateArrow;
    public Image bufferArrow;

    public bool initAutoInput = false;
    public int selectedInputString = 0;

    private float horizontalInput = 0;
    private float verticalInput = 0;

    private int buttonInputCode = 0;

    public int frameCounter = 0;

    public void OnMove(InputAction.CallbackContext context) {
        Vector2 vec2 = context.ReadValue<Vector2>();
        horizontalInput = vec2.x;
        verticalInput = vec2.y;
        Debug.Log("OnMove = " + vec2);
    }

        // This is bad. This leaves the input system to handle dead zone and the size of the cardinals vs diagonals
        // instead of handling control of it myself.
    //public void OnMove(InputAction.CallbackContext context) {
    //    Vector2 vec2 = context.ReadValue<Vector2>();
    //    horizontalInput = Mathf.RoundToInt(vec2.x);
    //    verticalInput = Mathf.RoundToInt(vec2.y);
    //    Debug.Log("OnMove = " + vec2);
    //}

    public void OnButtonA(InputAction.CallbackContext context) {
        Debug.Log("ButtonA performed: " + context.performed + " at frame(" + frameCounter + ")");
        SetButtonBit(context.performed, FightingGameInputCodeBut.A);
    }

    public void OnButtonB(InputAction.CallbackContext context) {
        Debug.Log("ButtonB performed: " + context.performed + " at frame(" + frameCounter + ")");
        SetButtonBit(context.performed, FightingGameInputCodeBut.B);
    }

    public void OnButtonC(InputAction.CallbackContext context) {
        Debug.Log("ButtonC performed: " + context.performed + " at frame(" + frameCounter + ")");
        SetButtonBit(context.performed, FightingGameInputCodeBut.C);
    }

    public void OnButtonD(InputAction.CallbackContext context) {
        Debug.Log("ButtonD performed: " + context.performed + " at frame(" + frameCounter + ")");
        SetButtonBit(context.performed, FightingGameInputCodeBut.D);
    }

    public void OnButtonS(InputAction.CallbackContext context) {
        Debug.Log("ButtonS performed: " + context.performed + " at frame(" + frameCounter + ")");
        SetButtonBit(context.performed, FightingGameInputCodeBut.S);
    }

    private void SetButtonBit(bool setTrue, FightingGameInputCodeBut bitMask) {
        buttonInputCode = buttonInputCode & ~((int)bitMask) | (setTrue ? (int)bitMask : 0);
    }

    private void ResetFrameCounter() {
        initAutoInput = true;
        frameCounter = 0;
    }

    public void Update() {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest1");
            selectedInputString = 0;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest2");
            selectedInputString = 1;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest3");
            selectedInputString = 2;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest4");
            selectedInputString = 3;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit5Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest5");
            selectedInputString = 4;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit6Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest6");
            selectedInputString = 5;
            ResetFrameCounter();
        }

        if (Keyboard.current.digit7Key.wasPressedThisFrame) {
            Debug.Log("pressed OnTest7");
            selectedInputString = 6;
            ResetFrameCounter();
        }

        if (horizontalInput == 0 && verticalInput == 0) {
            immediateArrow.enabled = false;
        }
        else {
            immediateArrow.enabled = true;
            //immediateArrow.GetComponent<Transform>().rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg - 90.0f);
            immediateArrow.GetComponent<Transform>().rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(-horizontalInput, verticalInput) * Mathf.Rad2Deg);
        }

        if (inputBuffer.GetLatestInput() == FightingGameInputCodeDir.Neutral || inputBuffer.GetLatestInput() == FightingGameInputCodeDir.None) {
            bufferArrow.enabled = false;
        }
        else {
            bufferArrow.enabled = true;
            bufferArrow.GetComponent<Transform>().rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2((((int) inputBuffer.GetLatestInput()) - 1) / 3 - 1, (((int) inputBuffer.GetLatestInput()) - 1) % 3 - 1) * Mathf.Rad2Deg - 90.0f);
        }

        FightingGameInputCodeDir fgInputCodeDir;
        if (autoInput && initAutoInput && frameCounter < selectableStrings[selectedInputString].direction.Length) {
            fgInputCodeDir = selectableStrings[selectedInputString].direction[frameCounter];
        }
        else {
            fgInputCodeDir = (FightingGameInputCodeDir) ((verticalInput + 1) * 3 + (horizontalInput + 1) + 1);
        }

        if (initAutoInput && frameCounter < selectableStrings[selectedInputString].buttonA.Length) {
            buttonInputCode += selectableStrings[selectedInputString].buttonA[frameCounter] ? (int) FightingGameInputCodeBut.A : 0;
        }
        if (initAutoInput && frameCounter < selectableStrings[selectedInputString].buttonB.Length) {
            buttonInputCode += selectableStrings[selectedInputString].buttonB[frameCounter] ? (int)FightingGameInputCodeBut.B : 0;
        }
        if (initAutoInput && frameCounter < selectableStrings[selectedInputString].buttonC.Length) {
            buttonInputCode += selectableStrings[selectedInputString].buttonC[frameCounter] ? (int)FightingGameInputCodeBut.C : 0;
        }
        if (initAutoInput && frameCounter < selectableStrings[selectedInputString].buttonD.Length) {
            buttonInputCode += selectableStrings[selectedInputString].buttonD[frameCounter] ? (int)FightingGameInputCodeBut.D : 0;
        }
        if (initAutoInput && frameCounter < selectableStrings[selectedInputString].buttonS.Length) {
            buttonInputCode += selectableStrings[selectedInputString].buttonS[frameCounter] ? (int)FightingGameInputCodeBut.S : 0;
        }

        inputBuffer.SetCurrentInputState(fgInputCodeDir, buttonInputCode);

        frameCounter++;
    }
}
