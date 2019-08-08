using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrameEnforcer : MonoBehaviour {

    public Action<int> updateAction = null;

    private float frameTime = 1f / 60.0f; // 1 sec over 60 frames
    private float elapsedTime = 0f;

    private int frameIndex = 0;

    public void Start() {
        elapsedTime = 0.0f;
        frameIndex = 0;
    }

    public void Update() {
        while (elapsedTime > frameTime) {
            updateAction.Invoke(frameIndex);

            frameIndex++;
            elapsedTime -= frameTime;
        }

        elapsedTime += Time.deltaTime;
    }

    public void SetUpdate(Action<int> updateAction) {
        Debug.Log("fnadkfdn");
        this.updateAction = updateAction;
        this.enabled = true;
    }
}
