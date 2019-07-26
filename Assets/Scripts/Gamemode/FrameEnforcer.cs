using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEnforcer : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        
    }

    private float frameTime = 1f / 60.0f; // 1 sec over 60 frames
    private float elapsedTime = 0f;

    void Update() {
        while (elapsedTime > frameTime) {
            elapsedTime -= frameTime;
        }

        elapsedTime += Time.deltaTime;
    }
}
