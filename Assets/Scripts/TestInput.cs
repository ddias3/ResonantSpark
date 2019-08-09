﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResonantSpark;

public class TestInput : MonoBehaviour {

    public FightingGameCharacter playerChar;

    public InputBuffer inputBuffer;

    public Image immediateArrow;
    public Image bufferArrow;

    private int horizontalInput = 0;
    private int verticalInput = 0;

    public void Start() {
        inputBuffer.character = playerChar;
    }

    public void Update() {
        horizontalInput = 0;
        verticalInput = 0;

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Debug.Log("Pause");
            inputBuffer.breakPoint = true;
        }

        if (Input.GetKey(KeyCode.W)) verticalInput += 1;

        if (Input.GetKey(KeyCode.S)) verticalInput += -1;

        if (Input.GetKey(KeyCode.A)) horizontalInput += -1;

        if (Input.GetKey(KeyCode.D)) horizontalInput += 1;

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

        inputBuffer.AddInput((FightingGameInputCodeDir) ((verticalInput + 1) * 3 + (horizontalInput + 1) + 1));
    }
}
